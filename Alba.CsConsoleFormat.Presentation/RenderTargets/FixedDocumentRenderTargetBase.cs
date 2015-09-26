using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WpfCanvas = System.Windows.Controls.Canvas;
using WpfSize = System.Windows.Size;

namespace Alba.CsConsoleFormat.Presentation
{
    public abstract class FixedDocumentRenderTargetBase : IRenderTarget
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

        public Brush Background { get; set; }

        public FontFamily FontFamily { get; set; }
        public double FontSize { get; set; }
        public FontStretch FontStretch { get; set; }
        public FontStyle FontStyle { get; set; }
        public FontWeight FontWeight { get; set; }

        protected FixedDocumentRenderTargetBase ()
        {
            Background = Brushes.Black;
            FontFamily = new FontFamily("Consolas");
            FontSize = 12;
            FontStretch = FontStretches.Normal;
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
        }

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
            }
        }

        public abstract void Render (IConsoleBufferSource buffer);

        protected void RenderToFixedDocument (IConsoleBufferSource buffer, FixedDocument document)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            WpfSize charSize = CharSize;
            WpfCanvas linesPanel = AddDocumentPage(document, buffer, charSize);
            RenderToCanvas(buffer, linesPanel, charSize);
        }

        protected static void RenderToCanvas (IConsoleBufferSource buffer, WpfCanvas linesPanel, WpfSize charSize)
        {
            ConsoleColor currentForeColor = (ConsoleColor)int.MaxValue;
            ConsoleColor currentBackColor = (ConsoleColor)int.MaxValue;
            TextBlock text = null;
            for (int iy = 0; iy < buffer.Height; iy++) {
                ConsoleChar[] charsLine = buffer.GetLine(iy);
                for (int ix = 0; ix < buffer.Width; ix++) {
                    ConsoleColor foreColor = charsLine[ix].ForegroundColor;
                    ConsoleColor backColor = charsLine[ix].BackgroundColor;
                    if (text == null || foreColor != currentForeColor || backColor != currentBackColor) {
                        if (text != null) {
                            text.Width = text.Text.Length * charSize.Width;
                            linesPanel.Children.Add(text);
                        }
                        text = new TextBlock {
                            Foreground = ConsoleBrushes[foreColor],
                            Background = ConsoleBrushes[backColor],
                            Height = charSize.Height,
                        };
                        WpfCanvas.SetLeft(text, ix * charSize.Width);
                        WpfCanvas.SetTop(text, iy * charSize.Height);

                        currentForeColor = foreColor;
                        currentBackColor = backColor;
                    }
                    char chr = charsLine[ix].Char;
                    LineChar lineChr = charsLine[ix].LineChar;
                    if (!lineChr.IsEmpty() && chr == '\0')
                        chr = buffer.GetLineChar(ix, iy);
                    text.Text += buffer.SafeChar(chr);
                }
            }
            if (text != null) {
                text.Width = text.Text.Length * charSize.Width;
                linesPanel.Children.Add(text);
            }
        }

        protected WpfCanvas AddDocumentPage (FixedDocument document, IConsoleBufferSource buffer, WpfSize charSize)
        {
            WpfCanvas linesPanel = CreateLinePanel(buffer, charSize);
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

        protected WpfCanvas CreateLinePanel (IConsoleBufferSource buffer, WpfSize charSize)
        {
            var linesPanel = new WpfCanvas {
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

        private WpfSize MeasureString (string text)
        {
            var formattedText = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), FontSize, Brushes.Transparent);
            return new WpfSize(formattedText.Width, formattedText.Height);
        }
    }
}