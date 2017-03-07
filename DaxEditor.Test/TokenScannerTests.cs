namespace DaxEditor.Test
{
    using System.Collections.Generic;
    using Babel.Parser;
    using NUnit.Framework;

    [TestFixture]
    public class TokenScannerTests
    {
        [Test]
        public void TokenScanner_OneLine()
        {
            string inputQuery = @"EVALUATE ROW(""a"", 1)";
            var tokenScanner = new TokenScanner(inputQuery, 0);
            var enumerator = tokenScanner.GetEnumerator();

            VerifyNextToken(enumerator, Tokens.KWEVALUATE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.FUNCTION);
            VerifyNextToken(enumerator, (Tokens)'(');
            VerifyNextToken(enumerator, Tokens.STRING);
            VerifyNextToken(enumerator, (Tokens)',');
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.NUMBER);
            VerifyNextToken(enumerator, (Tokens)')');

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenScanner_MultiLine()
        {
            string inputQuery = @"EVALUATE
ROW(""a"", 1)";
            var tokenScanner = new TokenScanner(inputQuery, 0);
            var enumerator = tokenScanner.GetEnumerator();

            VerifyNextToken(enumerator, Tokens.KWEVALUATE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.FUNCTION);
            VerifyNextToken(enumerator, (Tokens)'(');
            VerifyNextToken(enumerator, Tokens.STRING);
            VerifyNextToken(enumerator, (Tokens)',');
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.NUMBER);
            VerifyNextToken(enumerator, (Tokens)')');

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenScanner_CppStyleComment()
        {
            string inputQuery = @"EVALUATE T -- Comment";
            var tokenScanner = new TokenScanner(inputQuery, 0);
            var enumerator = tokenScanner.GetEnumerator();

            VerifyNextToken(enumerator, Tokens.KWEVALUATE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.TABLENAME);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);

            //? Why there is nothing after the comment?
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenScanner_NumberThatStarsWithDot()
        {
            string inputQuery = @"EVALUATE ROW(""a"", -.1)";
            var tokenScanner = new TokenScanner(inputQuery, 0);
            var enumerator = tokenScanner.GetEnumerator();

            VerifyNextToken(enumerator, Tokens.KWEVALUATE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.FUNCTION);
            VerifyNextToken(enumerator, (Tokens)'(');
            VerifyNextToken(enumerator, Tokens.STRING);
            VerifyNextToken(enumerator, (Tokens)',');
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, (Tokens)'-');
            VerifyNextToken(enumerator, Tokens.NUMBER);
            VerifyNextToken(enumerator, (Tokens)')');

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenScanner_CStyleCommentOneLine()
        {
            string inputQuery = @"EVALUATE T /* */";
            var tokenScanner = new TokenScanner(inputQuery, 0);
            var enumerator = tokenScanner.GetEnumerator();

            VerifyNextToken(enumerator, Tokens.KWEVALUATE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.TABLENAME);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        [Test]
        public void TokenScanner_CStyleCommentSeveralLines()
        {
            string inputQuery = @"/*
test of comment
*/
EVALUATE T";
            var tokenScanner = new TokenScanner(inputQuery, 0);
            var enumerator = tokenScanner.GetEnumerator();

            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);
            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);
            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);
            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);
            VerifyNextToken(enumerator, Tokens.LEX_COMMENT);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.KWEVALUATE);
            VerifyNextToken(enumerator, Tokens.LEX_WHITE);
            VerifyNextToken(enumerator, Tokens.TABLENAME);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsFalse(enumerator.MoveNext());
        }

        private static void VerifyNextToken(IEnumerator<TokenLocation> enumerator, Tokens expectedToken)
        {
            Assert.IsTrue(enumerator.MoveNext());

            TokenLocation tl;
            tl = enumerator.Current;
            Assert.IsNotNull(tl);
            Assert.AreEqual(expectedToken, tl.Token);
        }
    }
}
