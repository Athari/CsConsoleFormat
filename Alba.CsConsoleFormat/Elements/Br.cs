namespace Alba.CsConsoleFormat
{
    //[TrimSurroundingWhitespace]
    public class Br : InlineElement
    {
        protected override bool CanHaveChildren
        {
            get { return false; }
        }
    }
}