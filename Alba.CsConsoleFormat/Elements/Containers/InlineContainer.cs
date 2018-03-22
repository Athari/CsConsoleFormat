using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    internal sealed class InlineContainer : BlockElement
    {
        private InlineSequence _inlineSequence;
        private List<List<InlineSegment>> _lines;

        public InlineContainer([NotNull] BlockElement source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
          #if XAML
            DataContext = source.DataContext;
          #endif
            TextAlign = source.TextAlign;
            TextWrap = source.TextWrap;
            Parent = source;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width == 0)
                return new Size(0, 0);

            if (_inlineSequence == null) {
                _inlineSequence = new InlineSequence(this);
                foreach (InlineElement child in VisualChildren.Cast<InlineElement>())
                    child.GenerateSequence(_inlineSequence);
                _inlineSequence.ValidateStackSize();
            }
            _lines = new LineWrapper(this, availableSize).WrapSegments();
            return new Size(_lines.Select(GetLineLength).Max(), _inlineSequence.IsEmpty ? 0 : _lines.Count);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        protected override void RenderOverride(ConsoleBuffer buffer)
        {
            base.RenderOverride(buffer);
            if (_inlineSequence.IsEmpty)
                return;

            ConsoleColor color = EffectiveColor, background = EffectiveBackground;
            for (int y = 0; y < _lines.Count; y++) {
                List<InlineSegment> line = _lines[y];
                int length = GetLineLength(line);

                int offset = 0;
                if (TextAlign == TextAlign.Left || TextAlign == TextAlign.Justify)
                    offset = 0;
                else if (TextAlign == TextAlign.Center)
                    offset = (ActualWidth - length) / 2;
                else if (TextAlign == TextAlign.Right)
                    offset = ActualWidth - length;

                int x = offset;
                foreach (InlineSegment segment in line) {
                    if (segment.Color != null)
                        color = segment.Color.Value;
                    if (segment.Background != null)
                        background = segment.Background.Value;
                    if (segment.TextBuilder != null) {
                        string text = segment.ToString();
                        buffer.FillBackgroundRectangle(x, y, text.Length, 1, background);
                        buffer.DrawString(x, y, color, text);
                        x += text.Length;
                    }
                }
            }
        }

        private static int GetLineLength([NotNull] List<InlineSegment> line)
        {
            return line.Sum(s => s.TextLength);
        }

        private sealed class LineWrapper
        {
            private readonly InlineContainer _container;
            private readonly Size _availableSize;
            private List<List<InlineSegment>> _lines;
            private List<InlineSegment> _curLine;
            private InlineSegment _curSeg;
            private int _segPos;
            private int _curLineLength;

            // Last possible wrap info:
            private int _wrapPos;
            private CharInfo _wrapChar;
            private int _wrapSegmentIndex;

            public LineWrapper(InlineContainer container, Size availableSize)
            {
                _container = container;
                _availableSize = availableSize;
                _wrapPos = -1;
            }

            #if !NET_35
            [UsedImplicitly, ExcludeFromCodeCoverage]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            private IEnumerable<object> DebugLines => _lines.Select(l => new {
                text = string.Concat(l.Where(s => s.TextLength > 0).Select(s => s.ToString())),
                len = GetLineLength(l),
            });
            #endif

            private int AvailableWidth => _availableSize.Width;

            public List<List<InlineSegment>> WrapSegments()
            {
                _curLine = new List<InlineSegment>();
                _lines = new List<List<InlineSegment>> { _curLine };

                foreach (InlineSegment sourceSeg in _container._inlineSequence.Segments) {
                    if (sourceSeg.Color != null || sourceSeg.Background != null) {
                        _curLine.Add(sourceSeg);
                    }
                    else {
                        TextWrap textWrap = _container.TextWrap;
                        if (textWrap == TextWrap.NoWrap)
                            AppendTextSegmentNoWrap(sourceSeg);
                        else if (textWrap == TextWrap.CharWrap)
                            AppendTextSegmentCharWrap(sourceSeg);
                        else if (textWrap == TextWrap.WordWrap)
                            AppendTextSegmentWordWrap(sourceSeg);
                    }
                }

                return _lines;
            }

            private void AppendTextSegmentNoWrap([NotNull] InlineSegment sourceSeg)
            {
                _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                for (int i = 0; i < sourceSeg.Text.Length; i++) {
                    CharInfo c = CharInfo.From(sourceSeg.Text[i]);
                    if (c.IsNewLine)
                        StartNewLine();
                    else if (!c.IsZeroWidth && _curLineLength < AvailableWidth) {
                        _curSeg.TextBuilder.Append(c);
                        _curLineLength++;
                    }
                }
                AppendCurrentSegment();
            }

            private void AppendTextSegmentCharWrap([NotNull] InlineSegment sourceSeg)
            {
                _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                for (int i = 0; i < sourceSeg.Text.Length; i++) {
                    CharInfo c = CharInfo.From(sourceSeg.Text[i]);
                    Debug.Assert(_curLineLength == GetLineLength(_curLine) + _curSeg.TextLength);
                    if (!c.IsZeroWidth && _curLineLength >= AvailableWidth) {
                        // Proceed as if the current char is '\n', repeat with current char on the next iteration
                        // (unless current char is actually '\n', then it is consumed on wrap).
                        if (!c.IsNewLine) {
                            i--;
                            _curLineLength--;
                        }
                        c = CharInfo.From('\n');
                    }
                    if (c.IsNewLine)
                        StartNewLine();
                    else if (!c.IsZeroWidth) {
                        _curSeg.TextBuilder.Append(c);
                        _curLineLength++;
                    }
                }
                AppendCurrentSegment();
            }

            private void AppendTextSegmentWordWrap([NotNull] InlineSegment sourceSeg)
            {
                _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                _segPos = 0;
                for (int i = 0; i < sourceSeg.Text.Length; i++) {
                    CharInfo c = CharInfo.From(sourceSeg.Text[i]);
                    Debug.Assert(_curLineLength == GetLineLength(_curLine) + _curSeg.TextLength);
                    bool canAddChar = _curLineLength < AvailableWidth;
                    if (!canAddChar && !c.IsZeroWidth) {
                        if (c.IsConsumedOnWrap) {
                            c = CharInfo.From('\n');
                        }
                        else if (_wrapPos == -1) {
                            c = CharInfo.From('\n');
                            // Repeat with current char on the next iteration
                            i--;
                            _curLineLength--;
                            _segPos--;
                        }
                        else if (!c.IsNewLine) {
                            _curLine.Add(_curSeg);
                            WrapLine();
                        }
                    }
                    if (c.IsWrappable && (canAddChar || c.IsZeroWidth && !c.IsSoftHyphen)) {
                        _wrapPos = _segPos;
                        _wrapChar = c;
                        _wrapSegmentIndex = _curLine.Count;
                    }
                    if (c.IsNewLine) {
                        StartNewLine();
                    }
                    else if (!c.IsZeroWidth) {
                        _curSeg.TextBuilder.Append(c);
                        _curLineLength++;
                        _segPos++;
                    }
                }
                AppendCurrentSegment();
            }

            private void StartNewLine()
            {
                if (_curSeg != null && _curSeg.TextLength > 0) {
                    _curLine.Add(_curSeg);
                    _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                }
                _curLine = new List<InlineSegment>();
                _lines.Add(_curLine);
                _curLineLength = 0;
                _wrapPos = -1;
                _segPos = 0;
            }

            private void WrapLine()
            {
                // TODO MemPerf: Avoid calling StringBuilder.ToString (pass string builder instead of string to SplitWrappedText, remove text from it).
                string wrappedText = _curLine[_wrapSegmentIndex].ToString();
                SplitWrappedText(wrappedText, out string textBeforeWrap, out string textAfterWrap);

                // Put textBeforeWrap into wrappedSeg.
                if (wrappedText != textBeforeWrap) {
                    InlineSegment wrappedSeg = _curLine[_wrapSegmentIndex];
                    wrappedSeg.TextBuilder.Clear();
                    wrappedSeg.TextBuilder.Append(textBeforeWrap);
                }

                // Remember extra segments after the wrapped segment to add to the last line later. Total length guaranteed to be short enough.
                List<InlineSegment> segmentsAfterWrap = _curLine.Skip(_wrapSegmentIndex + 1).ToList();
                _curLine.RemoveRange(_wrapSegmentIndex + 1, segmentsAfterWrap.Count);

                // Start new line, ignoring current segment (it is being wrapped).
                _curSeg = null;
                StartNewLine();

                // Put textAfterWrap on new line.
                if (textAfterWrap.Length > 0) {
                    Debug.Assert(textAfterWrap.Length <= AvailableWidth);
                    InlineSegment nextSeg = InlineSegment.CreateWithBuilder(textAfterWrap.Length);
                    nextSeg.TextBuilder.Append(textAfterWrap);
                    _curLine.Add(nextSeg);
                    _curLineLength += textAfterWrap.Length;
                }

                _curLine.AddRange(segmentsAfterWrap);
                _curLineLength += GetLineLength(segmentsAfterWrap);

                _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
            }

            private void SplitWrappedText([NotNull] string wrappedText, [NotNull] out string textBeforeWrap, [NotNull] out string textAfterWrap)
            {
                if (_wrapChar.IsSpace) {
                    // "aaa bb" => "aaa" + "bb"
                    textBeforeWrap = wrappedText.Substring(0, _wrapPos);
                    textAfterWrap = wrappedText.Substring(_wrapPos + 1);
                }
                else if (_wrapChar.IsHyphen) {
                    // "aaa-bb" => "aaa-" + "bb"
                    textBeforeWrap = wrappedText.Substring(0, _wrapPos + 1);
                    textAfterWrap = wrappedText.Substring(_wrapPos + 1);
                }
                else if (_wrapChar.IsSoftHyphen) {
                    // "aaabb" => "aaa-" + "bb"
                    textBeforeWrap = wrappedText.Substring(0, _wrapPos) + "-";
                    textAfterWrap = wrappedText.Substring(_wrapPos);
                }
                else if (_wrapChar.IsZeroWidthSpace) {
                    // "aaabb" => "aaa" + "bb"
                    textBeforeWrap = wrappedText.Substring(0, _wrapPos);
                    textAfterWrap = wrappedText.Substring(_wrapPos);
                }
                else
                    throw new InvalidOperationException();
            }

            private void AppendCurrentSegment()
            {
                if (_curSeg.TextLength > 0)
                    _curLine.Add(_curSeg);
            }
        }

        private sealed class InlineSequence : IInlineSequence
        {
            private readonly Stack<InlineSegment> _formattingStack = new Stack<InlineSegment>();

            public List<InlineSegment> Segments { get; }

            [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific container type assumed.")]
            public InlineSequence([NotNull] InlineContainer container)
            {
                var initSegment = InlineSegment.CreateFromColors(container.EffectiveColor, container.EffectiveBackground);
                _formattingStack.Push(initSegment);
                Segments = new List<InlineSegment>();
                AddFormattingSegment();
            }

            public bool IsEmpty => Segments.All(s => s.TextLength == 0);

            public void AppendText(string text)
            {
                Segments.Add(InlineSegment.CreateFromText(text));
            }

            public void PushColor(ConsoleColor color)
            {
                _formattingStack.Push(InlineSegment.CreateFromColors(color, null));
                AddFormattingSegment();
            }

            public void PushBackground(ConsoleColor background)
            {
                _formattingStack.Push(InlineSegment.CreateFromColors(null, background));
                AddFormattingSegment();
            }

            public void PopFormatting()
            {
                _formattingStack.Pop();
                AddFormattingSegment();
            }

            public void ValidateStackSize()
            {
                if (_formattingStack.Count != 1)
                    throw new InvalidOperationException("Push and Pop calls during inline generation must be balanced.");
            }

            [SuppressMessage("ReSharper", "PossibleInvalidOperationException", Justification = "Initial segment is assigned effective values in the constructor which are not null.")]
            private void AddFormattingSegment()
            {
                InlineSegment lastSegment = Segments.LastOrDefault();
                if (lastSegment == null || lastSegment.Text != null) {
                    lastSegment = InlineSegment.CreateEmpty();
                    Segments.Add(lastSegment);
                }
                lastSegment.Color = _formattingStack.First(s => s.Color != null).Color.Value;
                lastSegment.Background = _formattingStack.First(s => s.Background != null).Background.Value;
            }
        }

        private sealed class InlineSegment
        {
            public ConsoleColor? Color { get; set; }
            public ConsoleColor? Background { get; set; }
            public string Text { get; }
            public StringBuilder TextBuilder { get; }

            private InlineSegment(string text, StringBuilder textBuilder)
            {
                Text = text;
                TextBuilder = textBuilder;
            }

            public int TextLength => TextBuilder?.Length ?? Text?.Length ?? 0;

            public static InlineSegment CreateEmpty() =>
                new InlineSegment(null, null);

            public static InlineSegment CreateFromColors(ConsoleColor? color, ConsoleColor? background) =>
                new InlineSegment(null, null) { Color = color, Background = background };

            public static InlineSegment CreateFromText([CanBeNull] string text) =>
                new InlineSegment(text?.Replace("\r", "") ?? "", null);

            // TODO MemPerf: Avoid calling InlineSegment.CreateWithBuilder (share string builder, try appending).
            public static InlineSegment CreateWithBuilder(int length) =>
                new InlineSegment(null, new StringBuilder(length));

            [ExcludeFromCodeCoverage]
            public override string ToString()
            {
                if (TextBuilder != null)
                    return TextBuilder.ToString();
                else if (Text != null)
                    return Text;
                else
                    return $"{(Color?.ToString() ?? "null")} {(Background?.ToString() ?? "null")}";
            }
        }

        private struct CharInfo
        {
            private readonly char _c;

            private CharInfo(char c)
            {
                _c = c;
            }

            public bool IsHyphen => _c == '-';
            public bool IsNewLine => _c == '\n';
            public bool IsSoftHyphen => _c == Chars.SoftHyphen;
            public bool IsSpace => _c == ' ';
            public bool IsConsumedOnWrap => _c == ' ' || _c == '\n' || _c == Chars.ZeroWidthSpace;
            public bool IsWrappable => _c == ' ' || _c == '-' || _c == Chars.SoftHyphen || _c == Chars.ZeroWidthSpace;
            public bool IsZeroWidth => _c == Chars.SoftHyphen || _c == Chars.ZeroWidthSpace;
            public bool IsZeroWidthSpace => _c == Chars.ZeroWidthSpace;

            public static CharInfo From(char c) => new CharInfo(c);

            public static implicit operator char(CharInfo self) => self._c;

            public override string ToString() => _c.ToString();
        }
    }
}