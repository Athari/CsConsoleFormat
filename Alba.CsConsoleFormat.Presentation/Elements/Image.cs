using System;
using System.Windows.Media;
using JetBrains.Annotations;
using WpfSize = System.Windows.Size;

namespace Alba.CsConsoleFormat.Presentation
{
    public class Image : BlockElement
    {
        [CanBeNull]
        public ImageSource Source { get; set; }

        public Stretch Stretch { get; set; } = Stretch.Uniform;
        public StretchDirection StretchDirection { get; set; } = StretchDirection.DownOnly;

        protected override Size MeasureOverride(Size availableSize)
        {
            return MeasureInternal(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return MeasureInternal(finalSize);
        }

        private Size MeasureInternal(Size inputSize)
        {
            if (Source == null)
                return new Size(0, 0);

            Size imageSize = new Size(Round(Source.Width), Round(Source.Height));
            WpfSize scaleFactor = ComputeScaleFactor(inputSize, imageSize, Stretch, StretchDirection);
            return new Size(Round(imageSize.Width * scaleFactor.Width), Round(imageSize.Height * scaleFactor.Height));
        }

        protected override void RenderOverride(ConsoleBuffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (Source == null)
                return;
            if (Background != null)
                buffer.FillBackgroundRectangle(new Rect(RenderSize), Background.Value);
            buffer.DrawImage(Source, new Rect(RenderSize));
        }

        private static WpfSize ComputeScaleFactor(Size availableSize, Size contentSize, Stretch stretch, StretchDirection stretchDirection)
        {
            bool isWidthInfinite = availableSize.IsWidthInfinite;
            bool isHeightInfinite = availableSize.IsHeightInfinite;

            if (stretch == Stretch.None || (isWidthInfinite && isHeightInfinite))
                return new WpfSize(1, 1);

            double scaleX = contentSize.Width == 0 ? 0 : (double)availableSize.Width / contentSize.Width;
            double scaleY = contentSize.Height == 0 ? 0 : (double)availableSize.Height / contentSize.Height;

            if (isWidthInfinite)
                scaleX = scaleY;
            else if (isHeightInfinite)
                scaleY = scaleX;
            else if (stretch == Stretch.Uniform)
                scaleX = scaleY = Math.Min(scaleX, scaleY);
            else if (stretch == Stretch.UniformToFill)
                scaleX = scaleY = Math.Max(scaleX, scaleY);

            Func<double, double, double> clip = null;
            if (stretchDirection == StretchDirection.UpOnly)
                clip = Math.Max;
            else if (stretchDirection == StretchDirection.DownOnly)
                clip = Math.Min;
            if (clip != null) {
                scaleX = clip(scaleX, 1);
                scaleY = clip(scaleY, 1);
            }

            return new WpfSize(scaleX, scaleY);
        }

        private static int Round(double value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}