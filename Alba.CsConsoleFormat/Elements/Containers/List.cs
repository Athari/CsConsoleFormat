using System;
using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;

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

        public override IEnumerable<Element> GenerateVisualElements()
        {
            Children.Replace(new[] {
                new Grid { Stroke = LineThickness.None }
                    .AddColumns(
                        GridLength.Auto,
                        GridLength.Star(1)
                    )
                    .AddChildren(
                        Children.Select((child, index) => new[] {
                            new Div { TextAlign = TextAlignment.Right }
                                .AddChildren(
                                    string.Format(EffectiveCulture, IndexFormat, StartIndex + index)
                                ),
                            child
                        })
                    )
            });
            return base.GenerateVisualElements();
        }
    }
}