namespace Alba.CsConsoleFormat.Fluent
{
    public static partial class Colors
    {
        public static void WriteLine(params object[] elements) =>
            ConsoleRenderer.RenderDocument(new Document { Children = { elements } });
    }
}