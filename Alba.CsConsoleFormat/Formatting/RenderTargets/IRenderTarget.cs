using JetBrains.Annotations;

namespace Alba.CsConsoleFormat
{
    public interface IRenderTarget
    {
        void Render ([NotNull] IConsoleBufferSource buffer);
    }
}