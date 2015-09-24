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
            Children.Replace(new[] { new GridBuilder().CreateGrid(this) });
            return base.GenerateVisualElements();
        }

        private class GridBuilder : DocumentBuilder
        {
            public Grid CreateGrid (List list) =>
                Create<Grid>()
                    .StrokeCell(LineThickness.None)
                    .AddColumns(
                        GridLength.Auto,
                        GridLength.Star(1)
                    )
                    .AddChildren(
                        Enumerable.Range(0, list.Children.Count).Select(index => new[] {
                            Create<Div>(string.Format(list.EffectiveCulture, list.IndexFormat, list.StartIndex + index))
                                .AlignText(TextAlignment.Right),
                            list.Children[index],
                        })
                    );
        }
    }
}