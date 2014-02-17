using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Alba.CsConsoleFormat
{
    public class ConsoleFormatter
    {
        public void Write (ConBody body)
        {
            WriteNode((dynamic)body);
        }

        private void WriteNode (XElement element)
        {
            using (GetFormatState(element)) {
                foreach (XNode node in element.Nodes()) {
                    WriteNode((dynamic)node);
                }
            }
        }

        private void WriteNode (XText element)
        {
            Console.Write(element.Value);
        }

        private IDisposable GetFormatState (XElement element)
        {
            var states = new List<FormatState>();
            ConsoleColor? back = GetAttrEnumValue<ConsoleColor>(element, AttrNames.Background);
            if (back != null)
                states.Add(new FormatStateBackground(back.Value));
            ConsoleColor? fore = GetAttrEnumValue<ConsoleColor>(element, AttrNames.Foreground);
            if (fore != null)
                states.Add(new FormatStateForeground(fore.Value));
            return states.Count > 0 ? new CompositeDisposable(states) : null;
        }

        private TEnum? GetAttrEnumValue<TEnum> (XElement element, XName attrName) where TEnum : struct
        {
            XAttribute attr = element.Attribute(attrName);
            return attr != null ? (TEnum)Enum.Parse(typeof(TEnum), attr.Value) : (TEnum?)null;
        }

        private abstract class FormatState : IDisposable
        {
            protected FormatState ()
            {
                GC.SuppressFinalize(this);
            }

            public abstract void Dispose ();
        }

        private class FormatStateBackground : FormatState
        {
            private readonly ConsoleColor _oldBackground;

            public FormatStateBackground (ConsoleColor background)
            {
                _oldBackground = Console.BackgroundColor;
                Console.BackgroundColor = background;
            }

            public override void Dispose ()
            {
                Console.BackgroundColor = _oldBackground;
            }
        }

        private class FormatStateForeground : FormatState
        {
            private readonly ConsoleColor _oldForeground;

            public FormatStateForeground (ConsoleColor foreground)
            {
                _oldForeground = Console.ForegroundColor;
                Console.ForegroundColor = foreground;
            }

            public override void Dispose ()
            {
                Console.ForegroundColor = _oldForeground;
            }
        }

        private class CompositeDisposable : IDisposable
        {
            private readonly List<IDisposable> _disposables;

            public CompositeDisposable (IEnumerable<IDisposable> disposables)
            {
                _disposables = disposables.ToList();
            }

            public void Dispose ()
            {
                foreach (IDisposable disposable in _disposables)
                    disposable.Dispose();
            }
        }
    }
}