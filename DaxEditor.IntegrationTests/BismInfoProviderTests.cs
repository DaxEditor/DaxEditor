using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AnalysisServices;
using System.IO;
using DaxEditor;
using System.Threading;
using DaxEditorSample.Test;

namespace DaxEditor.IntegrationTests
{
    [TestClass]
    public class BismInfoProviderTests
    {
        public static bool IsInitialized { get; private set; }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            IsInitialized = false;
            try
            {
                using (var server = new Server())
                {
                    server.Connect($"Data Source={Settings.SsasInstance}");
                    // Assumptions - the backups are stored in backup folder of the SSAS instance
                    server.Restore("db1.abf", "db1", true);
                    IsInitialized = true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Not initialized: " + exception.Message);
            }
        }

        [TestMethod]
        public void BismInfoProvider_GetSchemaSimple()
        {
            if (!IsInitialized)
            {
                return;
            }

            var props = new DaxDocumentPropertiesBase();
            props.ConnectionString = $"Data Source={Settings.SsasInstance};Catalog=db1";
            var provider = new BismInfoProvider(props);
            var schema = string.Empty;
            var updateEditorMargin = new StubIUpdateEditorMargin(updateSchema: i => schema = i);
            provider.SetUpdateEditorMargin(updateEditorMargin);
            provider.Connect();
            Thread.Sleep(10000); //TODO: make async and wait
            Assert.AreEqual(1, provider._tableDeclarations.Count());
            Assert.AreEqual(4, provider._tableMembersDeclarations["T1"].Count);
            Assert.AreEqual("[c]", provider._tableMembersDeclarations["T1"].First().Name);
            Assert.AreEqual("[M1]", provider._tableMembersDeclarations["T1"].Skip(1).First().Name);

            var expectedHtmlSchema = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">

<html>
<head>
    <title>Table schema</title>
    <style media=""screen"" type=""text/css"">
        body, table.daxtable {
            font-family: Consolas, ""Courier New"", Courier, monospace;
            font-size:small;
        }

        table.daxtable {
            border-collapse: collapse;
            float: left;
            margin: 20px 5px 10px 10px;
            border-color: #aaa;
            border-width: 1px;
            border-style: solid;
        }

        div.legend, div.table, div.column, div.calccolumn, div.measure, div.relationship, div.legendtext {
            width: 200px;
            margin-left: 50px;
            float:left;
        }

        div.table, th.table {
            background-color: #538135;
            color: white;
        }

        div.column, td.column {
            background-color: #b4c6e7;
            padding: 0px 10px;
        }

        div.calccolumn, td.calccolumn {
            background-color: #fee599;
            padding: 0px 10px;
        }

        div.measure, td.measure {
            background-color: #f7cbac;
            padding: 0px 10px;
        }

        div.relationship, td.relationship {
            background-color: #adb9ca;
            padding: 0px 10px;
        }

    </style>

</head>
<body>
    <div>
                <table class=""daxtable"">
            <thead>
                <tr>
                    <th class=""table"">T1</th>
                </tr>
            </thead>
            <tbody>

                <tr>
                    <td class=""column"">[c]</td>
                </tr>

                <tr>
                    <td class=""measure"">[M1]</td>
                </tr>

                <tr>
                    <td class=""measure"">[M Decimal]</td>
                </tr>

                <tr>
                    <td class=""measure"">[M Multiline]</td>
                </tr>

            </tbody>
        </table>

    </div>
    <div style=""clear: both; text-align: center;"">
        <div class=""legendtext"">Legend:</div>
        <div class=""table"">Table Name</div>
        <div class=""column"">Column Name</div>
        <div class=""measure"">Measure Name</div>
        <div class=""relationship"">1:M Relationship</div>
    </div>

</body>
</html>
";
            WindiffAssert.AreEqualNormalizedXmla(expectedHtmlSchema, schema);
        }
    }
}
