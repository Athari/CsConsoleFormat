using System;

namespace Alba.CsConsoleFormat.Generation
{
    public interface IElementBuilder
    {
        Element Element { get; }
        Type ElementType { get; }
    }

    public interface IElementBuilder<out T> : IElementBuilder
    {
        new T Element { get; }
    }
}