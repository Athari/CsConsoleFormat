using System;
using System.Collections.Generic;
using System.Text;

namespace Alba.CsConsoleFormat
{
    internal class InlineContainer : BlockElement
    {
        protected override Size MeasureOverride (Size availableSize)
        {
            int width = availableSize.Width;
            if (width == 0)
                return new Size(0, 0);

            var sb = new StringBuilder();
            foreach (InlineElement child in VisualChildren)
                sb.Append(child.GeneratedText);
            IList<string> generatedStrings = sb.Replace("\r", "").ToString().Split('\n');

            IList<string> actualStrings = new List<string>();
            int maxLineLength = 0;
            foreach (string generatedString in generatedStrings) {
                if (generatedString.Length <= width) {
                    actualStrings.Add(generatedString);
                    maxLineLength = Math.Max(maxLineLength, generatedString.Length);
                }
                else {
                    // TODO Add word wrapping
                    for (int i = 0; i < generatedString.Length; i += width)
                        actualStrings.Add(generatedString.Substring(i, Math.Min(generatedString.Length, i + width)));
                    maxLineLength = width;
                }
            }
            return new Size(maxLineLength, actualStrings.Count);
        }

        protected override Size ArrangeOverride (Size finalSize)
        {
            return finalSize;
        }
    }
}