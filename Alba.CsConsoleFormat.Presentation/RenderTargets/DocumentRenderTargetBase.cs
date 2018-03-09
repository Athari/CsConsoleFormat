using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using JetBrains.Annotations;
using static Alba.CsConsoleFormat.Presentation.FontDefaults;
using WpfSize = System.Windows.Size;

namespace Alba.CsConsoleFormat.Presentation
{
    public abstract class DocumentRenderTargetBase : IRenderTarget
    {
        private static readonly Dictionary<ConsoleColor, Brush> ConsoleBrushes = new Dictionary<ConsoleColor, Brush> {
            [ConsoleColor.Black] = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
            [ConsoleColor.DarkBlue] = new SolidColorBrush(Color.FromRgb(0, 0, 128)),
            [ConsoleColor.DarkGreen] = new SolidColorBrush(Color.FromRgb(0, 128, 0)),
            [ConsoleColor.DarkCyan] = new SolidColorBrush(Color.FromRgb(0, 128, 128)),
            [ConsoleColor.DarkRed] = new SolidColorBrush(Color.FromRgb(128, 0, 0)),
            [ConsoleColor.DarkMagenta] = new SolidColorBrush(Color.FromRgb(128, 0, 128)),
            [ConsoleColor.DarkYellow] = new SolidColorBrush(Color.FromRgb(128, 128, 0)),
            [ConsoleColor.Gray] = new SolidColorBrush(Color.FromRgb(192, 192, 192)),
            [ConsoleColor.DarkGray] = new SolidColorBrush(Color.FromRgb(128, 128, 128)),
            [ConsoleColor.Blue] = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
            [ConsoleColor.Green] = new SolidColorBrush(Color.FromRgb(0, 255, 0)),
            [ConsoleColor.Cyan] = new SolidColorBrush(Color.FromRgb(0, 255, 255)),
            [ConsoleColor.Red] = new SolidColorBrush(Color.FromRgb(255, 0, 0)),
            [ConsoleColor.Magenta] = new SolidColorBrush(Color.FromRgb(255, 0, 255)),
            [ConsoleColor.Yellow] = new SolidColorBrush(Color.FromRgb(255, 255, 0)),
            [ConsoleColor.White] = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
        };

        public Brush Background { get; set; } = DefaultConsoleBackground;

        public FontFamily FontFamily { get; set; } = DefaultFontFamily;
        public double FontSize { get; set; } = DefaultFontSize;
        public FontStretch FontStretch { get; set; } = DefaultFontStretch;
        public FontStyle FontStyle { get; set; } = DefaultFontStyle;
        public FontWeight FontWeight { get; set; } = DefaultFontWeight;

        internal DocumentRenderTargetBase()
        { }

        protected WpfSize CharSize
        {
            get
            {
                WpfSize charSize = MeasureString("i"), charSize2 = MeasureString("W");
                if (charSize != charSize2)
                    throw new InvalidOperationException($"Font family '{FontFamily}' is not monospace.");
                charSize.Width = Math.Ceiling(charSize.Width);
                charSize.Height = Math.Ceiling(charSize.Height);
                return charSize;

                WpfSize MeasureString(string text)
                {
                    var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                        new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Brushes.Transparent);
                    return new WpfSize(formattedText.Width, formattedText.Height);
                }
            }
        }

        public abstract void Render(IConsoleBufferSource buffer);

        // TODO Change 'protected' to 'private protected' in DocumentRenderTargetBase when R# starts supporting it.
        protected void RenderToFixedDocument([NotNull] IConsoleBufferSource buffer, [NotNull] FixedDocument document)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            WpfSize charSize = CharSize;
            Canvas linesPanel = AddDocumentPage(document, buffer, charSize);
            RenderToCanvas(buffer, linesPanel, charSize);
        }

        protected void RenderToFlowDocument([NotNull] IConsoleBufferSource buffer, [NotNull] FlowDocument document)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            var par = new Paragraph {
                Background = Background,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
                TextAlignment = TextAlignment.Left,
            };
            document.Blocks.Add(par);

            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;
            Run text = null;
            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);
                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleChar chr = charsLine[ix];
                    ConsoleColor foreColor = chr.ForegroundColor;
                    ConsoleColor backColor = chr.BackgroundColor;
                    if (text == null || foreColor != currentForeColor || backColor != currentBackColor) {
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                        AppendRunIfNeeded();
                        text = new Run {
                            Foreground = ConsoleBrushes[foreColor],
                            Background = ConsoleBrushes[backColor],
                        };
                    }
                    text.Text += chr.HasChar || chr.LineChar.IsEmpty ? chr.PrintableChar : buffer.GetLineChar(ix, iy);
                }
                AppendRunIfNeeded();
                if (iy + 1 < buffer.Height)
                    par.Inlines.Add(new LineBreak());
            }
            AppendRunIfNeeded();

            void AppendRunIfNeeded()
            {
                if (text == null)
                    return;
                par.Inlines.Add(text);
                text = null;
            }
        }

        protected static void RenderToCanvas([NotNull] IConsoleBufferSource buffer, Canvas linesPanel, WpfSize charSize)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;
            TextBlock text = null;
            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);
                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleChar chr = charsLine[ix];
                    ConsoleColor foreColor = chr.ForegroundColor;
                    ConsoleColor backColor = chr.BackgroundColor;
                    if (text == null || foreColor != currentForeColor || backColor != currentBackColor) {
                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                        AppendTextBlockIfNeeded();
                        text = new TextBlock {
                            Foreground = ConsoleBrushes[foreColor],
                            Background = ConsoleBrushes[backColor],
                            Height = charSize.Height,
                        };
                        Canvas.SetLeft(text, ix * charSize.Width);
                        Canvas.SetTop(text, iy * charSize.Height);
                    }
                    text.Text += chr.HasChar || chr.LineChar.IsEmpty ? chr.PrintableChar : buffer.GetLineChar(ix, iy);
                }
                AppendTextBlockIfNeeded();
            }
            AppendTextBlockIfNeeded();

            void AppendTextBlockIfNeeded()
            {
                if (text == null)
                    return;
                text.Width = text.Text.Length * charSize.Width;
                linesPanel.Children.Add(text);
                text = null;
            }
        }

        private Canvas AddDocumentPage([NotNull] FixedDocument document, [NotNull] IConsoleBufferSource buffer, WpfSize charSize)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            Canvas linesPanel = CreateLinePanel(buffer, charSize);
            document.Pages.Add(
                new PageContent {
                    Child = new FixedPage {
                        Width = buffer.Width * charSize.Width,
                        Height = buffer.Height * charSize.Height,
                        Background = Background,
                        Children = { linesPanel },
                    }
                });
            return linesPanel;
        }

        protected Canvas CreateLinePanel([NotNull] IConsoleBufferSource buffer, WpfSize charSize)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            var linesPanel = new Canvas {
                Width = buffer.Width * charSize.Width,
                Height = buffer.Height * charSize.Height,
                Background = Background,
            };
            TextBlock.SetFontFamily(linesPanel, FontFamily);
            TextBlock.SetFontSize(linesPanel, FontSize);
            TextBlock.SetFontStretch(linesPanel, FontStretch);
            TextBlock.SetFontStyle(linesPanel, FontStyle);
            TextBlock.SetFontWeight(linesPanel, FontWeight);
            return linesPanel;
        }
    }
}