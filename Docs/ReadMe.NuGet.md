![CsConsoleFormat Logo](Images/CsConsoleFormatIcon256.png)

*CsConsoleFormat: advanced formatting of console output for .NET*
=================================================================

* [**GitHub repository**](https://github.com/Athari/CsConsoleFormat)

CsConsoleFormat is a library for formatting text in console based on documents resembling a mix of WPF and HTML: tables, lists, paragraphs, colors, word wrapping, lines etc. Like this:

```xml
<Document>
    <Span Color="Red">Hello</Span>
    <Br/>
    <Span Color="Yellow">world!</Span>
</Document>
```

or like this:

```c#
new Document(
    new Span("Hello") { Color = ConsoleColor.Red },
    "\n",
    new Span("world!") { Color = ConsoleColor.Yellow }
);
```

or even like this:

```c#
Colors.WriteLine("Hello".Red(), "\n", "world!".Yellow());
```

Why?
====

.NET Framework includes only very basic console formatting capabilities. If you need to output a few strings, it's fine. If you want to output a table, you have to calculate column widths manually, often hardcode them. If you want to color output, you have to intersperse writing strings with setting and restoring colors. If you want to wrap words properly or combine all of the above...

The code quickly becomes an unreadable mess. It's just not fun! In GUI, we have MV*, bindings and all sorts of cool stuff. Writing console applications feels like returning to the Stone Age.

*CsConsoleFormat to the rescue!*

Imagine you have the usual Order, OrderItem and Customer classes. Let's create a document which prints the order. There're two major syntaxes, you can use either.

**XAML** (like WPF):

```xml
<Document xmlns="urn:alba:cs-console-format"
          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Span Background="Yellow" Text="Order #"/>
    <Span Text="{Get OrderId}"/>
    <Br/>
    <Span Background="Yellow" Text="Customer: "/>
    <Span Text="{Get Customer.Name}"/>

    <Grid Color="Gray">
        <Grid.Columns>
            <Column Width="Auto"/>
            <Column Width="*"/>
            <Column Width="Auto"/>
        </Grid.Columns>
        <Cell Stroke="Single Double" Color="White">Id</Cell>
        <Cell Stroke="Single Double" Color="White">Name</Cell>
        <Cell Stroke="Single Double" Color="White">Count</Cell>
        <Repeater Items="{Get OrderItems}">
            <Cell>
                <Span Text="{Get Id}"/>
            </Cell>
            <Cell>
                <Span Text="{Get Name}"/>
            </Cell>
            <Cell Align="Right">
                <Span Text="{Get Count}"/>
            </Cell>
        </Repeater>
    </Grid>
</Document>
```

```c#
// Assuming Order.xaml is stored as an Embedded Resource in the Views folder.
Document doc = ConsoleRenderer.ReadDocumentFromResource(GetType(), "Views.Order.xaml", Order);
ConsoleRenderer.RenderDocument(doc);
```

**C#** (like LINQ to XML):

```c#
using static System.ConsoleColor;

var headerThickness = new LineThickness(LineWidth.Double, LineWidth.Single);

var doc = new Document(
    new Span("Order #") { Color = Yellow }, Order.Id, "\n",
    new Span("Customer: ") { Color = Yellow }, Order.Customer.Name,
    new Grid {
        Color = Gray,
        Columns = { GridLength.Auto, GridLength.Star(1), GridLength.Auto },
        Children = {
            new Cell("Id") { Stroke = headerThickness },
            new Cell("Name") { Stroke = headerThickness },
            new Cell("Count") { Stroke = headerThickness },
            Order.OrderItems.Select(item => new[] {
                new Cell(item.Id),
                new Cell(item.Name),
                new Cell(item.Count) { Align = Align.Right },
            })
        }
    }
);

ConsoleRenderer.RenderDocument(doc);
```

**C#** (like npm/colors):

```c#
using Alba.CsConsoleFormat.Fluent;

Colors.WriteLine(
    "Order #".Yellow(), Order.Id, "\n",
    "Customer: ".Yellow(), Order.Customer.Name,
    // the rest is the same
);
```

Features
========

* **HTML-like elements**: paragraphs, spans, tables, lists, borders, separators.
* **Layouts**: grid, stacking, docking, wrapping, absolute.
* **Text formatting**: foreground and background colors, character wrapping, word wrapping.
* **Unicode formatting**: hyphens, soft hyphens, no-break hyphens, spaces, no-break spaces, zero-width spaces.
* **Multiple syntaxes** (see examples above):
    * **Like WPF**: XAML with one-time bindings, resources, converters, attached properties, loading documents from assembly resources.
    * **Like LINQ to XML**: C# with object initializers, setting attached properties via extension methods or indexers, adding children elements by collapsing enumerables and converting objects and strings to elements.
    * **Like npm/colors**: Limited to writing colored strings, but very concise. Can be combined with the general syntax above.
* **Drawing**: geometric primitives (lines, rectangles) using box-drawing characters, color transformations (dark, light), text, images.
* **Internationalization**: cultures are respected on every level and can be customized per-element.
* **Export** to many formats: ANSI text, unformatted text, HTML; RTF, XPF, WPF FixedDocument, WPF FlowDocument.
* **JetBrains ReSharper annotations**: CanBeNull, NotNull, ValueProvider, Pure etc.
* **WPF** document control, document converter.

Getting started
===============

1. Install NuGet package [Alba.CsConsoleFormat](https://www.nuget.org/packages/Alba.CsConsoleFormat) using Package Manager:

        PM> Install-Package Alba.CsConsoleFormat

    or .NET CLI:

        > dotnet add package Alba.CsConsoleFormat

2. Add `using Alba.CsConsoleFormat;` to your .cs file.

3. If you're going to use ASCII graphics on Windows, set `Console.OutputEncoding = Encoding.UTF8;`.

4. If you want to use XAML:

    1. Add XAML file to your project. Set its build action to "Embedded Resource".
    2. Load XAML using `ConsoleRenderer.ReadDocumentFromResource`.

5. If you want to use pure C#:

    1. Build a document in code starting with `Document` element as a root.

6. Call `ConsoleRenderer.RenderDocument` on the generated document.

Links
=====

TODO
