using Alba.CsConsoleFormat.Testing.FluentAssertions;
using FluentAssertions;
using Xunit;

namespace Alba.CsConsoleFormat.Tests
{
    public class InlineContainerTests : ElementTestsBase
    {
        [Fact]
        public void Empty()
        {
            var doc = new Document().AddChildren();

            GetRenderedText(doc, 3).Should().BeLines("");
        }

        [Fact]
        public void NoWrap()
        {
            var doc = new Document { TextWrap = TextWrapping.NoWrap }
                .AddChildren("a bc def");

            GetRenderedText(doc, 3).Should().BeLines("a b");
        }

        [Fact]
        public void NoWrapMultiLine()
        {
            var doc = new Document { TextWrap = TextWrapping.NoWrap }
                .AddChildren("a bc def\nghij klmno");

            GetRenderedText(doc, 5).Should().BeLines(
                "a bc ",
                "ghij ");
        }

        [Fact]
        public void NoWrapAlignCenter()
        {
            var doc = new Document { TextWrap = TextWrapping.NoWrap, TextAlign = TextAlignment.Center }
                .AddChildren("a\nbc\ndef\nghij\nklmno");

            GetRenderedText(doc, 4).Should().BeLines(
                " a  ",
                " bc ",
                "def ",
                "ghij",
                "klmn");
        }

        [Fact]
        public void NoWrapAlignRight()
        {
            var doc = new Document { TextWrap = TextWrapping.NoWrap, TextAlign = TextAlignment.Right }
                .AddChildren("a\nbc\ndef\nghij\nklmno");

            GetRenderedText(doc, 4).Should().BeLines(
                "   a",
                "  bc",
                " def",
                "ghij",
                "klmn");
        }

        [Fact]
        public void CharWrap()
        {
            var doc = new Document { TextWrap = TextWrapping.CharWrap }
                .AddChildren("a bc def ghij");

            GetRenderedText(doc, 4).Should().BeLines(
                "a bc",
                " def",
                " ghi",
                "j   ");
        }

        [Fact]
        public void CharWrapExact()
        {
            var doc = new Document { TextWrap = TextWrapping.CharWrap }
                .AddChildren("a bc def ghij123");

            GetRenderedText(doc, 4).Should().BeLines(
                "a bc",
                " def",
                " ghi",
                "j123");
        }

        [Fact]
        public void CharWrapMultiLine()
        {
            var doc = new Document { TextWrap = TextWrapping.CharWrap }
                .AddChildren("a bc def\nghij klmno");

            GetRenderedText(doc, 5).Should().BeLines(
                "a bc ",
                "def  ",
                "ghij ",
                "klmno");
        }

        [Fact]
        public void CharWrapMultiLineExact()
        {
            var doc = new Document { TextWrap = TextWrapping.CharWrap }
                .AddChildren("a bc def\nghij");

            GetRenderedText(doc, 4).Should().BeLines(
                "a bc",
                " def",
                "ghij");
        }

        [Fact]
        public void WordWrapSingleLine()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("abc");

            GetRenderedText(doc, 5).Should().BeLines("abc  ");
        }

        [Fact]
        public void WordWrapWithSpaces()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("a bc def ghijk lmnopq");

            GetRenderedText(doc, 5).Should().BeLines(
                "a bc ",
                "def  ",
                "ghijk",
                "lmnop",
                "q    ");
        }

        [Fact]
        public void WordWrapWithSpacesAlignRight()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap, TextAlign = TextAlignment.Right }
                .AddChildren("a bc def ghijk lmnopq");

            GetRenderedText(doc, 5).Should().BeLines(
                " a bc",
                "  def",
                "ghijk",
                "lmnop",
                "    q");
        }

        [Fact]
        public void WordWrapMultipleUnwrappable()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("a bcdefg hijklm");

            GetRenderedText(doc, 3).Should().BeLines(
                "a  ",
                "bcd",
                "efg",
                "hij",
                "klm");
        }

        [Fact]
        public void WordWrapWithHyphens()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("a-bc-def-ghijk-lmnopq");

            GetRenderedText(doc, 5).Should().BeLines(
                "a-bc-",
                "def- ",
                "ghijk",
                "-    ",
                "lmnop",
                "q    ");
        }

        [Fact]
        public void WordWrapWithHyphensAlignRight()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap, TextAlign = TextAlignment.Right }
                .AddChildren("a-bc-def-ghijk-lmnopq");

            GetRenderedText(doc, 5).Should().BeLines(
                "a-bc-",
                " def-",
                "ghijk",
                "    -",
                "lmnop",
                "    q");
        }

        [Fact]
        public void WordWrapWithSoftHyphens()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("a-bc-def-ghij-klmno-pqrstu".Replace('-', Chars.SoftHyphen));

            GetRenderedText(doc, 5).Replace(Chars.SoftHyphen, '-').Should().BeLines(
                "abc- ",
                "def- ",
                "ghij-",
                "klmno",
                "pqrst",
                "u    ");
        }

        [Fact]
        public void WordWrapWithSoftHyphensAlignRight()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap, TextAlign = TextAlignment.Right }
                .AddChildren("a-bc-def-ghij-klmno-pqrstu".Replace('-', Chars.SoftHyphen));

            GetRenderedText(doc, 5).Replace(Chars.SoftHyphen, '-').Should().BeLines(
                " abc-",
                " def-",
                "ghij-",
                "klmno",
                "pqrst",
                "    u");
        }

        [Fact]
        public void WordWrapSplitToChars()
        {
            // Tests wrapping of extra segments.
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("1", "2", Chars.SoftHyphen, "1", "2", "3");

            GetRenderedText(doc, 4).Replace(Chars.SoftHyphen, '-').Should().BeLines(
                "12- ",
                "123 ");
        }

        [Fact]
        public void WordWrapWithZeroWidthSpaces()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap }
                .AddChildren("a-bc-def-ghij-klmno-pqrstu".Replace('-', Chars.ZeroWidthSpace));

            GetRenderedText(doc, 5).Should().BeLines(
                "abc  ",
                "def  ",
                "ghij ",
                "klmno",
                "pqrst",
                "u    ");
        }

        [Fact]
        public void WordWrapWithZeroWidthSpacesAlignRight()
        {
            var doc = new Document { TextWrap = TextWrapping.WordWrap, TextAlign = TextAlignment.Right }
                .AddChildren("a-bc-def-ghij-klmno-pqrstu".Replace('-', Chars.ZeroWidthSpace));

            GetRenderedText(doc, 5).Should().BeLines(
                "  abc",
                "  def",
                " ghij",
                "klmno",
                "pqrst",
                "    u");
        }
    }
}