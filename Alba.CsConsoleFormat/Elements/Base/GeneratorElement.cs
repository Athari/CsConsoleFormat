namespace Alba.CsConsoleFormat
{
    public abstract class GeneratorElement : Element
    {
        protected GeneratorElement()
        { }

        protected GeneratorElement(params object[] children) : base(children)
        { }
    }
}