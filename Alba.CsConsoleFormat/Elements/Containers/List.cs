using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Alba.CsConsoleFormat.Framework.Collections;

namespace Alba.CsConsoleFormat
{
    public class List : BlockElement
    {
        internal const string DefaultIndexFormat = "{0}. ";

        public string IndexFormat { get; set; } = DefaultIndexFormat;
        public int StartIndex { get; set; } = 1;

        public override IEnumerable<Element> GenerateVisualElements ()
        {
            var grid = new Grid {
                AutoPosition = true,
                Stroke = LineThickness.None,
                Columns = {
                    new Column { Width = GridLength.Auto },
                    new Column { Width = GridLength.Star(1) },
                }
            };
            grid.Children.AddRange(GenerateCells(Children));
            Children.Replace(new[] { grid });
            return base.GenerateVisualElements();
        }

        [SuppressMessage ("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        private IEnumerable<Element> GenerateCells (IEnumerable<Element> elements)
        {
            int index = StartIndex;
            foreach (BlockElement child in elements) {
                yield return new Cell {
                    TextAlign = TextAlignment.Right,
                    Stroke = LineThickness.None,
                    Children = {
                        new Span(string.Format(IndexFormat, index++))
                    }
                };
                Grid.SetStroke(child, LineThickness.None);
                yield return child;
            }
        }
    }
}