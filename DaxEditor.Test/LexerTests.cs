// The project released under MS-PL license https://daxeditor.codeplex.com/license

using Babel.ParserGenerator;
using NUnit.Framework;

namespace DaxEditorSample.Test
{
    [TestFixture]
    public class LexerTests
    {
        [Test]
        public void GetText_SingleLine()
        {
            Babel.Parser.ErrorHandler handler = new Babel.Parser.ErrorHandler();
            Babel.Lexer.Scanner scanner = new Babel.Lexer.Scanner();
            scanner.Handler = handler;

            scanner.SetSourceText("CREATE MEASURE t[B]=Random()");
            var text = scanner.GetText(new LexLocation(1, 20, 1, 28));
            Assert.IsFalse(handler.Errors, "Error happened");
            Assert.AreEqual("Random()", text);
        }

        [Test]
        public void GetText_MultiLine()
        {
            Babel.Parser.ErrorHandler handler = new Babel.Parser.ErrorHandler();
            Babel.Lexer.Scanner scanner = new Babel.Lexer.Scanner();
            scanner.Handler = handler;

            scanner.SetSourceText(@"CREATE MEASURE t[B]=Random()
+ 1 +
2");
            var text = scanner.GetText(new LexLocation(1, 20, 3, 1));
            Assert.IsFalse(handler.Errors, "Error happened");
            Assert.AreEqual(@"Random()
+ 1 +
2", text);


        }

    }
}
