using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Alba.CsConsoleFormat
{
    internal class InlineContainer : BlockElement
    {
        //private List<string> _actualStrings;
        private InlineSequence _inlineSequence;
        private List<List<InlineSegment>> _lines;

        public InlineContainer (BlockElement source)
        {
            DataContext = source.DataContext;
            TextAlign = source.TextAlign;
            TextWrapping = source.TextWrapping;
            Parent = source;
        }

        protected override Size MeasureOverride (Size availableSize)
        {
            if (availableSize.Width == 0)
                return new Size(0, 0);

            if (_inlineSequence == null) {
                _inlineSequence = new InlineSequence(this);
                foreach (InlineElement child in VisualChildren.Cast<InlineElement>())
                    child.GenerateSequence(_inlineSequence);
            }
            _lines = new WrapContext(this, availableSize).WrapSegments();
            return new Size(_lines.Select(GetLineLength).Max(), _lines.Count);
        }


        protected override Size ArrangeOverride (Size finalSize)
        {
            return finalSize;
        }

        public override void Render (ConsoleRenderBuffer buffer)
        {
            base.Render(buffer);
            ConsoleColor color = EffectiveColor, bgColor = EffectiveBgColor;
            for (int y = 0; y < _lines.Count; y++) {
                List<InlineSegment> line = _lines[y];
                int length = GetLineLength(line);

                int offset = 0;
                if (TextAlign == HorizontalAlignment.Left || TextAlign == HorizontalAlignment.Stretch)
                    offset = 0;
                else if (TextAlign == HorizontalAlignment.Center)
                    offset = (ActualWidth - length) / 2;
                else if (TextAlign == HorizontalAlignment.Right)
                    offset = ActualWidth - length;

                int x = offset;
                foreach (InlineSegment segment in line) {
                    if (segment.Color != null)
                        color = segment.Color.Value;
                    if (segment.BgColor != null)
                        bgColor = segment.BgColor.Value;
                    if (segment.TextBuilder != null) {
                        string text = segment.ToString();
                        buffer.FillBackgroundRectangle(x, y, text.Length, 1, bgColor);
                        buffer.DrawString(x, y, color, text);
                        x += text.Length;
                    }
                }
            }
        }

        private static int GetLineLength (List<InlineSegment> line)
        {
            return line.Sum(s => s.TextLength);
        }

        private class WrapContext
        {
            private readonly InlineContainer _container;
            private readonly Size _availableSize;
            private List<List<InlineSegment>> _lines;
            private List<InlineSegment> _curLine;
            private InlineSegment _curSeg;

            public WrapContext (InlineContainer container, Size availableSize)
            {
                _container = container;
                _availableSize = availableSize;
            }

            private int AvailableWidth
            {
                get { return _availableSize.Width; }
            }

            public List<List<InlineSegment>> WrapSegments ()
            {
                _curLine = new List<InlineSegment>();
                _lines = new List<List<InlineSegment>> { _curLine };

                foreach (InlineSegment sourceSeg in _container._inlineSequence.Segments) {
                    if (sourceSeg.Color != null || sourceSeg.BgColor != null) {
                        _curLine.Add(sourceSeg);
                    }
                    else {
                        TextWrapping textWrapping = _container.TextWrapping;
                        if (textWrapping == TextWrapping.NoWrap)
                            AppendTextSegmentNoWrap(sourceSeg);
                        else if (textWrapping == TextWrapping.CharWrap)
                            AppendTextSegmentCharWrap(sourceSeg);
                        else if (textWrapping == TextWrapping.WordWrap)
                            AppendTextSegmentWordWrap(sourceSeg);
                    }
                }

                return _lines;
            }

            private void AppendTextSegmentNoWrap (InlineSegment sourceSeg)
            {
                _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                foreach (CharInfo c in sourceSeg.Text.Select(CharInfo.From)) {
                    if (c.IsNewLine)
                        StartNewLine();
                    else if (c.IsZeroWidth)
                        _curSeg.TextBuilder.Append(c);
                }
                AppendLastSegment();
            }

            private void AppendTextSegmentCharWrap (InlineSegment sourceSeg)
            {
                _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                for (int i = 0; i < sourceSeg.Text.Length; i++) {
                    CharInfo c = CharInfo.From(sourceSeg.Text[i]);

                    if (c.IsZeroWidth && _curSeg.TextLength >= AvailableWidth) {
                        i--; // Repeat with this character again.
                        c = CharInfo.From('\n');
                    }
                    if (c.IsNewLine)
                        StartNewLine();
                    else if (c.IsZeroWidth)
                        _curSeg.TextBuilder.Append(c);
                }
                AppendLastSegment();
            }

            private void AppendTextSegmentWordWrap (InlineSegment sourceSeg)
            {
                throw new NotImplementedException();
            }

            private void StartNewLine ()
            {
                if (_curSeg.TextLength > 0) {
                    _curLine.Add(_curSeg);
                    _curSeg = InlineSegment.CreateWithBuilder(AvailableWidth);
                }
                _curLine = new List<InlineSegment>();
                _lines.Add(_curLine);
            }

            private void AppendLastSegment ()
            {
                if (_curSeg.TextLength > 0)
                    _curLine.Add(_curSeg);
            }
        }

        private class InlineSequence : IInlineSequence
        {
            private readonly Stack<InlineSegment> _formattingStack = new Stack<InlineSegment>();

            public List<InlineSegment> Segments { get; private set; }

            public InlineSequence (InlineContainer container)
            {
                var initSegment = new InlineSegment {
                    Color = container.EffectiveColor,
                    BgColor = container.EffectiveBgColor,
                };
                _formattingStack.Push(initSegment);
                Segments = new List<InlineSegment>();
                AddFormattingSegment();
            }

            public void AppendText (string text)
            {
                Segments.Add(InlineSegment.CreateFromText(text));
            }

            public void PushColor (ConsoleColor color)
            {
                _formattingStack.Push(InlineSegment.CreateFromColors(color, null));
                AddFormattingSegment();
            }

            public void PushBgColor (ConsoleColor bgColor)
            {
                _formattingStack.Push(InlineSegment.CreateFromColors(null, bgColor));
                AddFormattingSegment();
            }

            public void PopFormatting ()
            {
                _formattingStack.Pop();
                AddFormattingSegment();
            }

            [SuppressMessage ("ReSharper", "PossibleInvalidOperationException", Justification = "Value is guaranteed not be not null.")]
            private void AddFormattingSegment ()
            {
                InlineSegment lastSegment = Segments.LastOrDefault();
                if (lastSegment == null || lastSegment.Text != null) {
                    lastSegment = new InlineSegment();
                    Segments.Add(lastSegment);
                }
                lastSegment.Color = _formattingStack.First(s => s.Color != null).Color.Value;
                lastSegment.BgColor = _formattingStack.First(s => s.BgColor != null).BgColor.Value;
            }
        }

        private class InlineSegment
        {
            public ConsoleColor? Color { get; set; }
            public ConsoleColor? BgColor { get; set; }
            public string Text { get; private set; }
            public StringBuilder TextBuilder { get; private set; }

            public int TextLength
            {
                get { return TextBuilder != null ? TextBuilder.Length : Text != null ? Text.Length : 0; }
            }

            public static InlineSegment CreateFromColors (ConsoleColor? color, ConsoleColor? bgColor)
            {
                return new InlineSegment { Color = color, BgColor = bgColor };
            }

            public static InlineSegment CreateFromText (string text)
            {
                return new InlineSegment { Text = text != null ? text.Replace("\r", "") : "" };
            }

            public static InlineSegment CreateWithBuilder (int length)
            {
                return new InlineSegment { TextBuilder = new StringBuilder(length) };
            }

            public override string ToString ()
            {
                if (TextBuilder != null)
                    return TextBuilder.ToString();
                else if (Text != null)
                    return Text;
                else
                    return (Color != null ? Color.ToString() : "null") + " " + (BgColor != null ? BgColor.ToString() : "null");
            }
        }

        private struct CharInfo
        {
            private readonly char _c;

            private CharInfo (char c)
            {
                _c = c;
            }

            public bool IsNewLine
            {
                get { return _c == '\n'; }
            }

            public bool IsZeroWidth
            {
                get { return _c != Chars.SoftHyphen && _c != Chars.ZeroWidthSpace; }
            }

            public static CharInfo From (char c)
            {
                return new CharInfo(c);
            }

            public static implicit operator char (CharInfo self)
            {
                return self._c;
            }
        }
    }
}