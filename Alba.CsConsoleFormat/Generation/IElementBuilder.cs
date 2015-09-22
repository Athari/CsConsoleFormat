using System;

namespace Alba.CsConsoleFormat.Generation
{
    public interface IElementBuilder
    {
        Element Element { get; }
        Type ElementType { get; }
    }
}