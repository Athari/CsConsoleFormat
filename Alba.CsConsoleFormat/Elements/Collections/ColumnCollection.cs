using System;
using System.Collections;

namespace Alba.CsConsoleFormat
{
    public class ColumnCollection : ElementCollection<Column>
    {
        public ColumnCollection(Element parent) : base(parent)
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
                    AddItem(new Column { Width = length });
                    break;
                case Column column:
                    AddItem(column);
                    break;
                default: {
                    int width;
                    try {
                        width = Convert.ToInt32(child);
                    }
                    catch (Exception e) when (e is FormatException || e is InvalidCastException || e is OverflowException) {
                        throw new ArgumentException($"Value of type '{child.GetType().Name}' cannot be converted to column.");
                    }
                    AddItem(new Column { Width = width == -1 ? GridLength.Auto : GridLength.Char(width) });
                    break;
                }
            }
        }
    }
}