using System;
using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;
using Alba.CsConsoleFormat.Generation;

namespace Alba.CsConsoleFormat
{
    public class List : BlockElement
    {
        internal const string DefaultIndexFormat = "{0}. ";

        private string _indexFormat = DefaultIndexFormat;

        public int StartIndex { get; set; } = 1;

        public string IndexFormat
        {
            get { return _indexFormat; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _indexFormat = value;
            }
        }

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
                        list.Children.Select((child, index) => new[] {
                            Create<Div>(string.Format(list.EffectiveCulture, list.IndexFormat, list.StartIndex + index))
                                .AlignText(TextAlignment.Right),
                            child,
                        })
                    );
        }
    }
}