using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Fluent
{
    public static class DocumentExts
    {
        public static void Render([NotNull] this Document @this, [CanBeNull] IRenderTarget target = null, Rect? renderRect = null) =>
            ConsoleRenderer.RenderDocument(@this, target, renderRect);

        public static string RenderToText([NotNull] this Document document, [NotNull] TextRenderTargetBase target, Rect? renderRect = null) =>
            ConsoleRenderer.RenderDocumentToText(document, target, renderRect);

        public static void RenderToBuffer([NotNull] this Document document, [NotNull] ConsoleBuffer buffer, Rect renderRect) =>
            ConsoleRenderer.RenderDocumentToBuffer(document, buffer, renderRect);
    }
}