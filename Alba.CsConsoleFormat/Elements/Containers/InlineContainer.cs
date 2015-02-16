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
            int availableWidth = availableSize.Width;
            if (availableWidth == 0)
                return new Size(0, 0);

            _lines = new List<List<InlineSegment>>();

            if (_inlineSequence == null) {
                _inlineSequence = new InlineSequence(this);
                foreach (InlineElement child in VisualChildren.Cast<InlineElement>())
                    child.GenerateSequence(_inlineSequence);
            }

            var curLine = new List<InlineSegment>();
            bool isEOL = false;
            _lines.Add(curLine);

            foreach (InlineSegment sourceSeg in _inlineSequence.Segments) {
                if (sourceSeg.Color != null || sourceSeg.BgColor != null) {
                    curLine.Add(sourceSeg);
                }
                else {
                    var curSeg = InlineSegment.CreateWithBuilder(availableWidth);
                    foreach (char c in sourceSeg.Text) {
                        if (TextWrapping == TextWrapping.NoWrap) {
                            if (c == '\n') {
                                curLine = new List<InlineSegment>();
                                _lines.Add(curLine);
                            }
                            else if (c != Chars.SoftHyphen && c != Chars.ZeroWidthSpace) {
                                curSeg.TextBuilder.Append(c);
                            }
                        }
                    }
                    if (curSeg.TextBuilder.Length > 0)
                        curLine.Add(curSeg);
                }
            }

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
            return line.Sum(s => s.TextBuilder != null ? s.TextBuilder.Length : s.Text != null ? s.Text.Length : 0);
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
                Segments = new List<InlineSegment> { initSegment };
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
                InlineSegment lastSegment = Segments.Last();
                if (lastSegment.Text != null) {
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
    }
}