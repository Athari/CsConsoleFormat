using System;
using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat.Framework.Collections;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public class List : BlockElement
    {
        private const string DefaultIndexFormat = "{0}. ";

        private string _indexFormat = DefaultIndexFormat;

        public int StartIndex { get; set; } = 1;

        public List()
        { }

        public List(params object[] children) : base(children)
        { }

        [NotNull]
        public string IndexFormat
        {
            get => _indexFormat;
            set => _indexFormat = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override IEnumerable<Element> GenerateVisualElements()
        {
            Children.Replace(new[] {
                new Grid {
                    Stroke = LineThickness.None,
                    Columns = { GridLength.Auto, GridLength.Star(1) },
                    Children = {
                        Children.Select((child, index) => new[] {
                            new Div {
                                TextAlign = TextAlign.Right,
                                Children = { string.Format(EffectiveCulture, IndexFormat, StartIndex + index) }
                            },
                            child
                        })
                    }
                }
            });
            return base.GenerateVisualElements();
        }
    }
}