using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Resources;
using WpfCanvas = System.Windows.Controls.Canvas;
using WpfSize = System.Windows.Size;

namespace Alba.CsConsoleFormat.Presentation.Controls
{
    [ContentProperty ("Document")]
    public class ConsoleView : Control
    {
        private static readonly DependencyPropertyKey ContentPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Content), typeof(object), typeof(ConsoleView), new PropertyMetadata(null));
        public static readonly DependencyProperty ContentProperty = ContentPropertyKey.DependencyProperty;
        public static readonly DependencyProperty ConsoleWidthProperty = DependencyProperty.Register(
            nameof(ConsoleWidth), typeof(int), typeof(ConsoleView), new PropertyMetadata(80, RenderPropertyChanged));
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(
            nameof(Document), typeof(Document), typeof(ConsoleView), new PropertyMetadata(RenderPropertyChanged));
        public static readonly DependencyProperty DocumentSourceProperty = DependencyProperty.Register(
            nameof(DocumentSource), typeof(Uri), typeof(ConsoleView), new PropertyMetadata(DocumentPropertyChanged));

        static ConsoleView ()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(typeof(ConsoleView)));
            DataContextProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(DocumentPropertyChanged));
            FontFamilyProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(RenderPropertyChanged));
            FontSizeProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(RenderPropertyChanged));
            FontStretchProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(RenderPropertyChanged));
            FontStyleProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(RenderPropertyChanged));
            FontWeightProperty.OverrideMetadata(typeof(ConsoleView), new FrameworkPropertyMetadata(RenderPropertyChanged));
        }

        public object Content
        {
            private get { return GetValue(ContentProperty); }
            set { SetValue(ContentPropertyKey, value); }
        }

        public int ConsoleWidth
        {
            get { return (int)GetValue(ConsoleWidthProperty); }
            set { SetValue(ConsoleWidthProperty, value); }
        }

        public Document Document
        {
            get { return (Document)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        public Uri DocumentSource
        {
            get { return (Uri)GetValue(DocumentSourceProperty); }
            set { SetValue(DocumentSourceProperty, value); }
        }

        private static void RenderPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (ConsoleView)d;
            @this.UpdateView();
        }

        private static void DocumentPropertyChanged (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var @this = (ConsoleView)d;
            if (e.NewValue == null)
                return;
            StreamResourceInfo resourceInfo = Application.GetResourceStream(@this.DocumentSource);
            if (resourceInfo == null)
                return;
            using (resourceInfo.Stream)
                @this.Document = ConsoleRenderer.ReadDocumentFromStream(resourceInfo.Stream, @this.DataContext);
            @this.UpdateView();
        }

        private void UpdateView ()
        {
            Document document = Document;
            if (document == null) {
                Content = null;
                return;
            }
            var target = new CanvasRenderTarget {
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStretch = FontStretch,
                FontStyle = FontStyle,
                FontWeight = FontWeight,
            };
            try {
                ConsoleRenderer.RenderDocument(document, target, new Rect(0, 0, ConsoleWidth, Size.Infinity));
            }
            catch (InvalidOperationException ex) {
                Content = ex.Message;
                return;
            }
            Content = target.Canvas;
        }

        private class CanvasRenderTarget : DocumentRenderTargetBase
        {
            public WpfCanvas Canvas { get; private set; }

            public override void Render (IConsoleBufferSource buffer)
            {
                WpfSize charSize = CharSize;
                Canvas = CreateLinePanel(buffer, charSize);
                RenderToCanvas(buffer, Canvas, charSize);
            }
        }
    }
}