﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
#if SYSTEM_XAML
using System.Xaml;
#elif PORTABLE_XAML
using Portable.Xaml;
#endif

namespace Alba.CsConsoleFormat
{
    public static class ConsoleRenderer
    {
        public static Size ConsoleBufferSize => new Size(Console.BufferWidth, Console.BufferHeight);

        public static Size ConsoleLargestWindowSize => new Size(Console.LargestWindowWidth, Console.LargestWindowHeight);

        public static Rect DefaultRenderRect => new Rect(0, 0, Console.BufferWidth, Size.Infinity);

        public static Point ConsoleCursorPosition
        {
            get => new Point(Console.CursorLeft, Console.CursorTop);
            set
            {
                Console.CursorLeft = value.X;
                Console.CursorTop = value.Y;
            }
        }

        public static Rect ConsoleWindowRect
        {
            get => new Rect(Console.WindowLeft, Console.WindowTop, Console.WindowWidth, Console.WindowHeight);
            set
            {
                Console.SetWindowPosition(value.Left, value.Top);
                Console.SetWindowSize(value.Width, value.Height);
            }
        }

        #if XAML

        [MustUseReturnValue]
        public static TElement ReadElementFromStream<TElement>([NotNull] Stream stream,
            [CanBeNull] object dataContext, [CanBeNull] XamlElementReaderSettings settings = null)
            where TElement : Element, new()
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (settings == null)
                settings = new XamlElementReaderSettings();
            var context = new XamlSchemaContext(
                settings.ReferenceAssemblies,
                new XamlSchemaContextSettings {
                    SupportMarkupExtensionsWithDuplicateArity = true,
                });
            var readerSettings = new XamlXmlReaderSettings {
                ProvideLineInfo = true,
                CloseInput = settings.CloseInput,
            };
            var writerSettings = new XamlObjectWriterSettings {
                RootObjectInstance = new TElement { DataContext = dataContext },
            };
            using (var xamlReader = new XamlXmlReader(stream, context, readerSettings))
            using (var xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext, writerSettings)) {
                XamlServices.Transform(xamlReader, xamlWriter, false);
                return (TElement)xamlWriter.Result;
            }
        }

        [MustUseReturnValue]
        public static TElement ReadElementFromResource<TElement>([NotNull] Type type, [NotNull] string resourceName,
            [CanBeNull] object dataContext, [CanBeNull] XamlElementReaderSettings settings = null)
            where TElement : Element, new()
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (resourceName == null)
                throw new ArgumentNullException(nameof(resourceName));
            return ReadElementFromResource<TElement>(type.GetAssembly(), $"{type.Namespace}.{resourceName}", dataContext, settings);
        }

        [MustUseReturnValue]
        public static TElement ReadElementFromResource<TElement>([NotNull] Assembly assembly, [NotNull] string resourceName,
            [CanBeNull] object dataContext, [CanBeNull] XamlElementReaderSettings settings = null)
            where TElement : Element, new()
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            if (resourceName == null)
                throw new ArgumentNullException(nameof(resourceName));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
                if (stream == null)
                    throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly '{assembly.GetName().Name}'");
                return ReadElementFromStream<TElement>(stream, dataContext, settings);
            }
        }

        [MustUseReturnValue]
        public static Document ReadDocumentFromStream([NotNull] Stream stream,
            [CanBeNull] object dataContext, [CanBeNull] XamlElementReaderSettings settings = null)
        {
            return ReadElementFromStream<Document>(stream, dataContext, settings);
        }

        [MustUseReturnValue]
        public static Document ReadDocumentFromResource([NotNull] Type type, [NotNull] string resourceName,
            [CanBeNull] object dataContext, [CanBeNull] XamlElementReaderSettings settings = null)
        {
            return ReadElementFromResource<Document>(type, resourceName, dataContext, settings);
        }

        [MustUseReturnValue]
        public static Document ReadDocumentFromResource([NotNull] Assembly assembly, [NotNull] string resourceName,
            [CanBeNull] object dataContext, [CanBeNull] XamlElementReaderSettings settings = null)
        {
            return ReadElementFromResource<Document>(assembly, resourceName, dataContext, settings);
        }

        #endif // XAML

        public static void RenderDocument([NotNull] Document document, [CanBeNull] IRenderTarget target = null, Rect? renderRect = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (target == null)
                target = new ConsoleRenderTarget();
            if (renderRect == null)
                renderRect = DefaultRenderRect;
            var buffer = new ConsoleBuffer(renderRect.Value.Size.Width);
            RenderDocumentToBuffer(document, buffer, renderRect.Value);
            target.Render(buffer);
        }

        public static string RenderDocumentToText([NotNull] Document document, [NotNull] TextRenderTargetBase target, Rect? renderRect = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (renderRect == null)
                renderRect = DefaultRenderRect;
            var buffer = new ConsoleBuffer(renderRect.Value.Size.Width);
            RenderDocumentToBuffer(document, buffer, renderRect.Value);
            target.Render(buffer);
            return target.OutputText;
        }

        public static void RenderDocumentToBuffer([NotNull] Document document, [NotNull] ConsoleBuffer buffer, Rect renderRect)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (document.LineCharRenderer != null)
                buffer.LineCharRenderer = document.LineCharRenderer;
            document.GenerateVisualTree();
            document.Measure(renderRect.Size);
            document.Arrange(new Rect(renderRect.Position, document.DesiredSize));
            RenderElement(document, buffer, new Vector(0, 0), document.LayoutClip);
        }

        private static void RenderElement([NotNull] BlockElement element, ConsoleBuffer buffer, Vector parentOffset, Rect renderRect)
        {
            if (element.Visibility != Visibility.Visible || element.RenderSize.IsEmpty)
                return;

            Vector offset = parentOffset + element.ActualOffset;
            Rect clip = new Rect(element.RenderSize).Intersect(element.LayoutClip).Offset(offset).Intersect(renderRect);

            buffer.Offset = offset;
            buffer.Clip = clip;
            element.Render(buffer);

            foreach (BlockElement childElement in element.VisualChildren.OfType<BlockElement>())
                RenderElement(childElement, buffer, offset, renderRect);
        }
    }
}