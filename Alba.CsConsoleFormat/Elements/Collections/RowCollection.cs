using System;
using System.Collections;

namespace Alba.CsConsoleFormat
{
    internal class RowCollection : ElementCollection<Row>
    {
        public RowCollection(Element parent) : base(parent)
        { }

        public override void Add(object child)
        {
            switch (child) {
                case null:
                    break;
                case IEnumerable enumerable:
                    foreach (object subchild in enumerable)
                        Add(subchild);
                    break;
                case GridLength length:
                    AddItem(new Row { Height = length });
                    break;
                case Row Row:
                    AddItem(Row);
                    break;
                default: {
                    int width;
                    try {
                        width = Convert.ToInt32(child);
                    }
                    catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException) {
                        throw new ArgumentException($"Value of type '{child.GetType().Name}' cannot be converted to row.");
                    }
                    AddItem(new Row { Height = width == -1 ? GridLength.Auto : GridLength.Char(width) });
                    break;
                }
            }
        }
    }
}