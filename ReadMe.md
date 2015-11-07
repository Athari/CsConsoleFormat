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
new Document()
    .AddChildren(
        new Span("Hello") { Color = ConsoleColor.Red },
        "\n",
        new Span("world!") { Color = ConsoleColor.Yellow }
    );
```

Why?
====

.NET Framework includes only very basic console formatting capabilities. If you need to output a few strings, it's fine. If you want to output a table, you have to calculate column widths manually, often hardcode them. If you want to color output, you have to intersperse writing strings with setting and restoring colors. If you want to wrap words properly or combine all of the above...

The code quickly becomes an unreadable mess. It's just not fun! In GUI, we have MV*, bindings and all sorts of cool stuff. Writing console applications feels like returning to the Stone Age.

*CsConsoleFormat to the rescue!*

Imagine you have usual Order, OrderItem and Customer classes. Let's create a document which prints the order. There're three syntaxes, you can use any of them in any combination.

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
        <Cell Stroke="Single Wide" Color="White">Id</Cell>
        <Cell Stroke="Single Wide" Color="White">Name</Cell>
        <Cell Stroke="Single Wide" Color="White">Count</Cell>
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

var headerThickness = new LineThickness(LineWidth.Single, LineWidth.Wide);

var doc = new Document()
    .AddChildren(
        new Span("Order #") { Color = Yellow },
        Order.Id,
        "\n",
        new Span("Customer: ") { Color = Yellow },
        Order.Customer.Name,

        new Grid { Color = Gray }
            .AddColumns(
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Star(1) },
                new Column { Width = GridLength.Auto }
            )
            .AddChildren(
                new Cell { Stroke = headerThickness }
                    .AddChildren("Id"),
                new Cell { Stroke = headerThickness }
                    .AddChildren("Name"),
                new Cell { Stroke = headerThickness }
                    .AddChildren("Count"),
                Order.OrderItems.Select(item => new[] {
                    new Cell()
                        .AddChildren(item.Id),
                    new Cell()
                        .AddChildren(item.Name),
                    new Cell { Align = HorizontalAlignment.Right }
                        .AddChildren(item.Count),
                })
            )
    );

ConsoleRenderer.RenderDocument(doc);
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
* **Drawing**: geometric primitives (lines, rectangles) using box-drawing characters, color transformations (dark, light), text.
* **Internationalization**: cultures are respected on every level and can be customized per-element.
* **Export** to many formats: ANSI text, unformatted text, HTML; RTF, XPF, WPF FixedDocument, WPF FlowDocument (requires WPF).
* **JetBrains R# annotations**: CanBeNull, NotNull, ValueProvider.
* **WPF** document control, document converter, image importer (pre-alpha).

Getting started
===============

TODO

Which syntax to choose?
=======================

**XAML** (like WPF) forces clear separation of views and models which is good thing. However, it isn't strongly typed, so it's easy to get runtime error. Syntax wise it's a combination of XML verbosity (`<Grid><Grid.Columns><Column/></Grid.Columns></Grid>`) and conciseness of short enums (`Color="White"`) and converters (`Stroke="Single Wide"`).

XAML library in Mono is currently very buggy. If you want to build a cross-platform application, using XAML may be problematic. However, if you need to support only Windows and are experienced in WPF, XAML should feel natural.

XAML is only partially supported by Visual Studio + ReSharper: syntax highlighting and code completion work, but almost the whole document is highlighted as containing numerous errors.

**C#** (like LINQ to XML) allows performing all sorts of transformations with objects right in the code, thanks to LINQ and collapsing of enumerables when adding children elements. When using C# 6, which supports `using static`, accessing some of enumerations can be shortened. The only place with loose typing is adding of children using `AddChildren(params object[])` extension method (which is optional).

Building documents in the code is fully supported, but currently there's a bug in ReSharper 9 which causes large documents to consume lots of CPU in Visual Studio 2015.

License
=======
Copyright © 2014 Alexander Prokhorov

Licensed under the [Apache License, Version 2.0](License.md) (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

<http://www.apache.org/licenses/LICENSE-2.0>

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Some parts of the library are based on ConsoleFramework © Igor Kostomin under MIT license.

Links
=====

* Related projects:
    * [ConsoleFramework](http://elw00d.github.io/consoleframework/) — full-featured cross-platform console user interface framework. Using ConsoleFramework, you can create interactive user interface, but its formatting capabilities are limited.