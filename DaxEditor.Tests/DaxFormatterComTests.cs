using System;
using NUnit.Framework;

namespace DaxEditor.Tests
{
    [TestFixture]
    public class DaxFormatterComTests
    {
        [Test]
        public void DaxFormatterCom_Trivial()
        {
            var input = @"
EVALUATE   T";
            var expected = @"EVALUATE
T

";
            var actual = DaxFormatterCom.Format(input, DaxFormatterCom.FormattingCulture.US);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DaxFormatterCom_SyntaxError()
        {
            var input = @"T[A] T[B]";
            var expected = @"Exception from www.daxformatter.com:
  Message: Syntax error, expected: =, :=
  Line: 0
  Column: 5";
            try
            {
                var actual = DaxFormatterCom.Format(input, DaxFormatterCom.FormattingCulture.US);
                Assert.Fail("Exception expected");
            }
            catch(Exception e)
            {
                var actual = e.Message;
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void DaxFormatterCom_Comments()
        {
            var input = @"/**** 
multi line comment
***/
  -- single line comment
EVALUATE   T";
            var expected = @"/**** 
multi line comment
***/
-- single line comment
EVALUATE
T

";
            var actual = DaxFormatterCom.Format(input, DaxFormatterCom.FormattingCulture.US);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DaxFormatterCom_Unicode()
        {
            var input = @"EVALUATE ROW(""Мама мыла раму"", 1)";
            var expected = @"EVALUATE
ROW ( ""Мама мыла раму"", 1 )

";
            var actual = DaxFormatterCom.Format(input, DaxFormatterCom.FormattingCulture.US);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DaxFormatterCom_JsonEncodingDecoding()
        {
            var input = @"Evaluate Row(""1"", ""	<TabChar>'/\"")";
            var expected = @"EVALUATE
ROW ( ""1"", ""	<TabChar>'/\"" )

";
            var actual = DaxFormatterCom.Format(input, DaxFormatterCom.FormattingCulture.US);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DaxFormatterCom_NotTrivialQuery()
        {
            var input = @"EVALUATE ROW(""Cumulative Quantity"", 
CALCULATE (     SUM ( Transactions[Quantity] ),     FILTER (         ALL ( 'Date'[Date] ),         'Date'[Date] <= MAX ( 'Date'[Date] )     ) ))";
            var expected = @"EVALUATE
ROW (
    ""Cumulative Quantity"", CALCULATE (
        SUM ( Transactions[Quantity] ),
        FILTER ( ALL ( 'Date'[Date] ), 'Date'[Date] <= MAX ( 'Date'[Date] ) )
    )
)

";
            var actual = DaxFormatterCom.Format(input, DaxFormatterCom.FormattingCulture.US);
            Assert.AreEqual(expected, actual);
        }
    }
}
