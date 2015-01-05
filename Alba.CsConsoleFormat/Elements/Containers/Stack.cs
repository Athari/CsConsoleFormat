namespace Alba.CsConsoleFormat
{
    public class Stack : ContainerElement
    {
        public Orientation Orientation { get; set; }

        public Stack ()
        {
            Orientation = Orientation.Vertical;
        }
    }
}