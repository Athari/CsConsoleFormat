using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Generation;

namespace Alba.CsConsoleFormat
{
    public class List : BlockElement
    {
        internal const string DefaultIndexFormat = "{0}. ";

        public string IndexFormat { get; set; } = DefaultIndexFormat;
        public int StartIndex { get; set; } = 1;

        public override IEnumerable<Element> GenerateVisualElements ()
        {
            var builder = new DocumentBuilder();
            Grid grid = builder.CreateGrid()
                .StrokeCell(LineThickness.None)
                .AddColumns(
                    builder.CreateColumn(GridLength.Auto),
                    builder.CreateColumn(GridLength.Star(1))
                )
                .AddChildren(
                    Enumerable.Range(0, Children.Count).Select(index => new[] {
                        builder.Create<Div>(string.Format(IndexFormat, StartIndex + index))
                            .AlignText(TextAlignment.Right),
                        Children[index],
                    })
                );
            Children.Replace(new[] { grid });
            return base.GenerateVisualElements();
        }
    }
}