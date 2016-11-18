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
        public static string _pathToBinDirectory;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _pathToBinDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            using(Server srv = new Server())
            {
                srv.Connect(string.Format("Data Source={0}", Settings.SsasInstance));
                srv.Restore("db1.abf", "db1", true);  // Assumptions - the backups are stored in backup folder of the SSAS instance
            }
        }

        [TestMethod]
        public void BismInfoProvider_GetSchemaSimple()
        {
            var props = new DaxDocumentPropertiesBase();
            props.ConnectionString = string.Format("Data Source={0};Catalog=db1", Settings.SsasInstance);
            var bismInfoProvider = new BismInfoProvider(props);
            string schema = string.Empty;
            var updateEditorMargin = new StubIUpdateEditorMargin(updateSchema: i => schema = i);
            bismInfoProvider.SetUpdateEditorMargin(updateEditorMargin);
            bismInfoProvider.Connect();
            Thread.Sleep(10000); //TODO: make async and wait
            Assert.AreEqual(1, bismInfoProvider._tableDeclarations.Count());
            Assert.AreEqual(4, bismInfoProvider._tableMembersDeclarations["T1"].Count);
            Assert.AreEqual("[c]", bismInfoProvider._tableMembersDeclarations["T1"].First().Name);
            Assert.AreEqual("[M1]", bismInfoProvider._tableMembersDeclarations["T1"].Skip(1).First().Name);

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
