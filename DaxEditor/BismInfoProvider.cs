// The project released under MS-PL license https://daxeditor.codeplex.com/license

extern alias ssasmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Babel;
using ssasmd::Microsoft.AnalysisServices;
using System.Threading;
using System.Data;
using System.Diagnostics;
using adomd = Microsoft.AnalysisServices.AdomdClient;
using DaxEditor.GeneratorSource;
using System.ComponentModel;
using System.IO;
using Microsoft.VisualStudio.Package;
using System.Xml.Linq;
using System.Reflection;

namespace DaxEditor
{
    public sealed class BismInfoProvider : ICompletionDataSnaphotProvider, IDisposable
    {
        #region Private static members

        // Schema constants
        // Functions:
        private const string FUNCTIONS_SCHEMA = "MDSCHEMA_FUNCTIONS";
        private const string FUNCTIONS_RESTRICTION_ORIGIN = "ORIGIN";
        private const int FUNCTIONS_FIELD_NAME = 0;
        private const int FUNCTIONS_FIELD_DESCRIPTION = 1;
        private const int FUNCTIONS_FIELD_CATEGORY = 5;     // Called "Interface_name" in the rowset
        private const int FUNCTIONS_FIELD_ORIGIN = 2;
        private const string FUNCTIONS_RELATION_PARAMETERINFO = "rowsetTablePARAMETERINFO";
        private const int FUNCTIONS_ORIGIN_RELATIONAL = 3;
        private const int FUNCTIONS_ORIGIN_SCALAR = 4;

        // Parameter info nested rowset fields:
        private const int PARAMETERS_FIELD_NAME = 1;
        private const int PARAMETERS_FIELD_DESCRIPTION = 2;
        private const int PARAMETERS_FIELD_OPTIONAL = 3;
        private const int PARAMETERS_FIELD_REPEATABLE = 4;

        #endregion

        #region Private members

        /// <summary>
        /// DAX Document Properties (server name, database name)
        /// </summary>
        private IDaxDocumentProperties _daxDocumentProperties;

        /// <summary>
        /// AdomdConnection to SSAS tabular database
        /// </summary>
        adomd.AdomdConnection _adomdConn;

        /// <summary>
        /// Database object that has up-to-date structure
        /// </summary>
        private Database _database;

        /// <summary>
        /// String representation of RowNumber column name
        /// </summary>
        private IEnumerable<string> _rowNumberColumnNames;

        private const string BeginTableTemplate = @"        <table class=""daxtable"">
            <thead>
                <tr>
                    <th class=""table"">{0}</th>
                </tr>
            </thead>
            <tbody>
";
        private const string RowTemplate = @"
                <tr>
                    <td class=""{0}"">{1}</td>
                </tr>
";
        private const string EndTableTemplate = @"
            </tbody>
        </table>
";
        private const string TablesPlaceholder = @"<!-- tables -->";


        // Private cache

        /// <summary>
        /// Declarations for tables.
        /// </summary>
        internal List<Declaration> _tableDeclarations;

        /// <summary>
        /// Declarations for tables.
        /// </summary>
        internal List<Declaration> _measureDeclarations;

        /// <summary>
        /// Declarations for DAX functions
        /// </summary>
        internal List<Declaration> _daxFunctionDeclarations;

        /// <summary>
        /// List of DAX Functions/Methods
        /// </summary>
        internal List<Method> _daxMethods;

        /// <summary>
        /// Declarations for table child objects (columns, measures)
        /// </summary>
        internal Dictionary<string, List<Declaration>> _tableMembersDeclarations;

        /// <summary>
        /// Schema in html of the database, if available
        /// </summary>
        internal string _htmlSchema;

        /// <summary>
        /// Way to tell UI that result needs to be updated
        /// </summary>
        internal IUpdateEditorMargin _updateEditorMargin;

        /// <summary>
        /// Is Bism provider currently connecting to the database?
        /// </summary>
        internal bool _isConnecting;

        private CompletionDataSnapshot _completionDataSnapshot = new CompletionDataSnapshot();

        #endregion Private members

        #region Public properties

        /// <summary>
        /// Gets or sets the database object
        /// </summary>
        public Database Database
        {
            set
            {
                this._database = value;
            }

            get
            {
                return this._database;
            }
        }

        #endregion Public properties

        #region Constructor

        /// <summary>
        /// Initializes an instance of BismInfoProvider with specified DAX document properties.
        /// </summary>
        /// <param name="daxDocumentProperties">DAX document properties holding server and database names.</param>
        public BismInfoProvider(IDaxDocumentProperties daxDocumentProperties)
        {
            this._daxDocumentProperties = daxDocumentProperties;

            this._database = null;

            this._rowNumberColumnNames = new string[] { "RowNumber", "__XL_RowNumber" };

            // Initialize empty cache
            this._tableDeclarations = null;
            this._measureDeclarations = null;
            this._daxFunctionDeclarations = null;
            this._tableMembersDeclarations = null;
            this._htmlSchema = null;
        }

        #endregion Constructor

        #region Public methods

        public void Dispose()
        {
            _adomdConn.Close();
        }

        /// <summary>
        /// Checks if DAX Function information is available or not
        /// </summary>
        /// <returns></returns>
        public bool IsDaxFunctionInformationAvailable()
        {
            lock (this)
            {
                return (this._daxFunctionDeclarations != null);
            }
        }
        
        /// <summary>
        /// Extracts table names from database object
        /// </summary>
        /// <returns>Returns list of table names</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when database object is null.</exception>
        public List<String> GetTableNames()
        {
            if (null == this._database)
                throw new InvalidOperationException("Current database is NULL. GetTableNames function cannot be called when database object is NULL");

            List<string> tableNames = new List<string>();

            foreach (Dimension dim in _database.Dimensions)
            {
                tableNames.Add(dim.Name);
            }

            return tableNames;
        }

        /// <summary>
        /// Extracts column names of the specified table from database object
        /// </summary>
        /// <param name="tableName">Table name that owns columns function should return</param>
        /// <returns>Returns list of column names</returns>
        /// <exception cref="System.ArgumentException">Thrown when table name is not found in database.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when database object is null.</exception>
        public List<String> GetColumnNames(string tableName)
        {
            if (null == this._database)
                throw new InvalidOperationException("Current database is NULL. GetColumnNames function cannot be called when database object is NULL");

            List<string> columnNames = new List<string>();

            Dimension dim = this._database.Dimensions.FindByName(tableName);
            if (null == dim)
                throw new ArgumentException(string.Format("Table '{0}' was not found in database {1}", tableName, this._database.Name), "tableName");

            foreach (DimensionAttribute dimAttr in dim.Attributes)
            {
                if (_rowNumberColumnNames.Any(i => { return string.Equals(i, dimAttr.Name, StringComparison.CurrentCultureIgnoreCase); }))
                    continue; // Hide the RowNumber column

                columnNames.Add(dimAttr.Name);
            }

            return columnNames;
        }
        

        /// <summary>
        /// Extracts measure names from database object.
        /// </summary>
        /// <param name="tableName">Specifies a table that will limit measure names to that specific one.</param>
        /// <returns>Returns list of measure names specific to a table or all of them if tableName is empty.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when database object is null.</exception>
        public IEnumerable<string> GetMeasureNames(string tableName)
        {
            if (null == this._database)
                throw new InvalidOperationException("Current database is NULL. GetMeasureNames function cannot be called when database object is NULL");

            var measureNames = new List<string>();

            if (this._database.Cubes.Count > 0)
            {
                var cube = this._database.Cubes[0];
                if (cube.MdxScripts.Count > 0)
                {
                    var mdxScrit = cube.MdxScripts[0];
                    foreach (Command mdxCommand in mdxScrit.Commands)
                    {
                        if (!string.IsNullOrEmpty(mdxCommand.Text))
                        {
                            var measures = MeasuresContainer.ParseDaxScript(mdxCommand.Text).Measures;
                            if (string.IsNullOrEmpty(tableName))
                                measureNames.AddRange(measures.Select(i => i.TableName + i.Name));
                            else
                                measureNames.AddRange(measures.Where(i => string.Equals(i.TableName, tableName, StringComparison.CurrentCultureIgnoreCase)).Select(i => i.TableName + i.Name));
                        }
                    }
                }
            }

            return measureNames;
        }

        /// <summary>
        /// Obtain the list of columns/measures declarations for the specified table. Return measure declarations in case of empty table.
        /// </summary>
        /// <param name="tableName">Table name specifying the owner of member objects.</param>
        /// <returns>Returns list of declarations for column/measure objects.</returns>
        /// <exception cref="System.ArgumentException">Thrown when table name is not found in database.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when database object is null or no valid information is found on it.</exception>
        public List<Declaration> GetTableMemberDeclarations(string tableName)
        {
            if (this._database == null)
                throw new InvalidOperationException("Current database is NULL. GetColumnNames function cannot be called when database object is NULL");

            List<Declaration> memberDeclarations = null;
            lock (this)
            {   // make sure the cache is not modified while we return requested Declaration
                if (this._tableMembersDeclarations == null)
                    throw new InvalidOperationException("No information is available in declarations cache.");

                if (string.IsNullOrEmpty(tableName))
                {
                    memberDeclarations = new List<Declaration>();
                    foreach (var tableMemberDeclaration in _tableMembersDeclarations)
                    {
                        foreach (var decl in tableMemberDeclaration.Value)
                        {
                            var fullDeclaration = new Declaration(decl.Description, string.Format("{0}[{1}]", tableMemberDeclaration.Key, decl.DisplayText), decl.Glyph, string.Format("{0}[{1}]", tableMemberDeclaration.Key, decl.DisplayText));
                            memberDeclarations.Add(fullDeclaration);
                        }
                    }
                }
                else
                {
                    foreach (var tableMemberDeclaration in this._tableMembersDeclarations)
                    {
                        if (string.Equals(tableMemberDeclaration.Key, tableName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            memberDeclarations = tableMemberDeclaration.Value;
                            break;
                        }
                    }
                }
            }
            Debug.Assert(memberDeclarations != null, string.Format("No member information was found for table '{0}'.", tableName));

            return memberDeclarations;
        }


        /// <summary>
        /// Returns table declarations from the declaration cache
        /// </summary>
        /// <returns>Returns Declarations object</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when database object is null or no valid information is found on it.</exception>
        public List<Declaration> GetTableDeclarations()
        {
            if (null == this._database)
                throw new InvalidOperationException("Current database is NULL. GetTableNames function cannot be called when database object is NULL");

            List<Declaration> tableDeclarations = null;
            lock (this)
            {   // make sure the cache is not modified while we return requested Declaration
                if (this._tableDeclarations == null)
                    throw new InvalidOperationException("No information is available in declarations cache.");

                tableDeclarations = this._tableDeclarations;
            }
            return tableDeclarations;
        }

        /// <summary>
        /// Returns measure declarations from the declaration cache
        /// </summary>
        /// <returns>Returns Declarations object</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when database object is null or no valid information is found on it.</exception>
        public List<Declaration> GetMeasureDeclarations()
        {
            if (null == this._database)
                throw new InvalidOperationException("Current database is NULL. GetTableNames function cannot be called when database object is NULL");

            List<Declaration> measureDeclarations = null;
            lock (this)
            {   // make sure the cache is not modified while we return requested Declaration
                if (this._measureDeclarations == null)
                    throw new InvalidOperationException("No information is available in declarations cache.");

                measureDeclarations = this._measureDeclarations;
            }
            return measureDeclarations;
        }

        /// <summary>
        /// Returns function declarations from the declaration cache
        /// </summary>
        /// <returns></returns>
        public List<Declaration> GetDaxFunctionDeclarations()
        {
            List<Declaration> daxFunctionDeclarations = null;
            lock (this)
            {   // make sure the cache is not modified while we return requested Declaration
                if (this._daxFunctionDeclarations == null)
                    throw new InvalidOperationException("No information is available in declarations cache.");

                daxFunctionDeclarations = this._daxFunctionDeclarations;
            }
            return daxFunctionDeclarations;
        }

        /// <summary>
        /// Returns DAX Functions from the cache
        /// </summary>
        /// <returns></returns>
        public List<Method> GetDaxMethods()
        {
            List<Method> daxMethods = null;
            lock (this)
            {   // make sure the cache is not modified while we return requested Declaration
                if (this._daxMethods == null)
                    throw new InvalidOperationException("No information is available in declarations cache.");

                daxMethods = this._daxMethods;
            }
            return daxMethods;
        }

        /// <summary>
        /// Refresh cache of declaraions
        /// </summary>
        public void RefreshDeclarationsCache()
        {
            LogLine("Begin refreshing declarations cache.");

            ConstructServerLevelDeclarations();

            // Update the database object
            try
            {
                Server srv = new Server();
                srv.Connect(this._daxDocumentProperties.ConnectionString);
                this._database = srv.Databases[_daxDocumentProperties.DatabaseName];
            }
            catch (Exception e)
            {
                this._database = null;
                ClearDeclarationsCache();
                LogLine("Error while refreshing declarations cache:" + e.ToString());
                return;
            }

            // Update declarations
            if (this._database != null)
            {
                this._database.Refresh(true);// FULL
            }

            ConstructSchemaBasedDeclarations();

            if (!string.IsNullOrEmpty(_htmlSchema))
                _updateEditorMargin.UpdateSchema(_htmlSchema);

            LogLine("End refreshing declarations cache.");
        }

        #endregion Public methods

        #region Protected methods

        private void LogLine(string message, bool switchToLogWindow = false)
        {
            _updateEditorMargin.WriteLog(DateTime.Now.ToLongTimeString() + " " + message + Environment.NewLine, false);
        }

        private void LogError(string message)
        {
            LogLine(message, true);
        }

        /// <summary>
        /// Clears declarations cache.
        /// </summary>
        private void ClearDeclarationsCache()
        {
            lock (this)
            {
                this._tableDeclarations = null;
                this._measureDeclarations = null;
                this._daxFunctionDeclarations = null;
                this._tableMembersDeclarations = null;
                this._htmlSchema = null;
            }
        }

        /// <summary>
        /// Constructs declarations cache based on database object
        /// </summary>
        private void ConstructSchemaBasedDeclarations()
        {
            LogLine("Begin constructing schema based declarations.");
            List<Declaration> tableDeclarations = new List<Declaration>();
            List<Declaration> measureDeclarations = new List<Declaration>();
            Dictionary<string, List<Declaration>> tableMembersDeclarations = new Dictionary<string, List<Declaration>>();

            List<string> tableList = new List<string>();
            List<string> columnList = new List<string>();
            List<string> measureList = new List<string>();

            if (null == this._database)
            {
                ClearDeclarationsCache();
                LogLine("ERROR: Database is not defined.");
                return;
            }

            foreach (Dimension dim in _database.Dimensions)
            {
                // Set table declarations
                tableDeclarations.Add(new Babel.Declaration(dim.Description, dim.Name, Babel.LanguageService.TABLE_GLYPH_INDEX, dim.Name));
                tableList.AddRange(GenerateValidTableNames(dim.Name));

                // Obtain column declarations for current table
                List<Declaration> columnDeclarationsCurrentTable = new List<Declaration>();
                foreach (DimensionAttribute dimAttr in dim.Attributes)
                {
                    // Skip RowNumber columns
                    if (_rowNumberColumnNames.Any(i => { return string.Equals(i, dimAttr.Name, StringComparison.CurrentCultureIgnoreCase); }))
                        continue;

                    var columnShortName = "[" + dimAttr.Name + "]";

                    if (dimAttr.Source is ExpressionBinding)
                    {
                        // use calc column glyph
                        columnDeclarationsCurrentTable.Add(new Babel.Declaration(dimAttr.Description, dimAttr.Name, Babel.LanguageService.CALC_COLUMN_GLYPH_INDEX, columnShortName));
                    }
                    else
                    {
                        // use base column glyph
                        columnDeclarationsCurrentTable.Add(new Babel.Declaration(dimAttr.Description, dimAttr.Name, Babel.LanguageService.BASE_COLUMN_GLYPH_INDEX, columnShortName));
                    }

                    GenerateValidTableNames(dim.Name).ForEach(i => columnList.Add(string.Format("{0}{1}", i, columnShortName)));
                }

                tableMembersDeclarations.Add(dim.Name, columnDeclarationsCurrentTable);
            }

            // Obtain measure declarations and distribute them in table member declarations
            if (this._database.Cubes.Count > 0)
            {
                var cube = this._database.Cubes[0];
                if (cube.MdxScripts.Count > 0)
                {
                    var mdxScript = cube.MdxScripts[0];
                    foreach (Command mdxCommand in mdxScript.Commands)
                    {
                        if (!string.IsNullOrEmpty(mdxCommand.Text))
                        {
                            var measures = MeasuresContainer.ParseDaxScript(mdxCommand.Text).Measures;
                            foreach (var measure in measures)
                            {
                                // Add measure declaration to the global collection
                                var measureShortName = "[" + measure.Name + "]";
                                measureDeclarations.Add(new Babel.Declaration(measure.Expression, measure.Name, Babel.LanguageService.MEASURE_GLYPH_INDEX, measureShortName));

                                GenerateValidTableNames(measure.TableName).ForEach(i => measureList.Add(string.Format("{0}{1}", i, measureShortName)));

                                // Add the measure declaration to the table member declaration list
                                if (tableMembersDeclarations.ContainsKey(measure.TableName))
                                {
                                    tableMembersDeclarations[measure.TableName].Add(new Babel.Declaration(measure.Expression, measure.Name, Babel.LanguageService.MEASURE_GLYPH_INDEX, measureShortName));
                                }
                            }
                        }
                    }
                }
            }
            // Create HTML table
            string htmlSchemaTemplate;
            using (var textStreamReader = new StreamReader(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("DaxEditor.Resources.ScemaTemplate.htm")))
            {
                htmlSchemaTemplate = textStreamReader.ReadToEnd();
            }
            var htmlSchemaBuilder = new StringBuilder();
            foreach (var td in tableMembersDeclarations)
            {
                htmlSchemaBuilder.AppendFormat(BeginTableTemplate, HtmlEncode(td.Key));
                foreach (var decl in td.Value)
                {
                    if (_rowNumberColumnNames.Any(i => { return string.Equals(i, decl.Name, StringComparison.CurrentCultureIgnoreCase); }))
                        continue;

                    switch (decl.Glyph)
                    {
                        case Babel.LanguageService.BASE_COLUMN_GLYPH_INDEX:
                            htmlSchemaBuilder.AppendFormat(RowTemplate, "column", HtmlEncode(decl.Name));
                            break;
                        case Babel.LanguageService.CALC_COLUMN_GLYPH_INDEX:
                            htmlSchemaBuilder.AppendFormat(RowTemplate, "calcolumn", HtmlEncode(decl.Name));
                            break;
                        case Babel.LanguageService.MEASURE_GLYPH_INDEX:
                            htmlSchemaBuilder.AppendFormat(RowTemplate, "measure", HtmlEncode(decl.Name));
                            break;
                    }
                }
                htmlSchemaBuilder.Append(EndTableTemplate);
            }
            htmlSchemaTemplate = htmlSchemaTemplate.Replace(TablesPlaceholder, htmlSchemaBuilder.ToString());

            // Commit the newly created declarations into the cache
            lock (this)
            {
                this._tableDeclarations = tableDeclarations;
                this._measureDeclarations = measureDeclarations;
                this._tableMembersDeclarations = tableMembersDeclarations;
                this._htmlSchema = htmlSchemaTemplate;

                _completionDataSnapshot = new CompletionDataSnapshot(_completionDataSnapshot, tableList, columnList, measureList);
            }

            LogLine("End constructing schema based declarations.");
        }

        /// <summary>
        /// Generate valid table names.  If table name does is alpha numeric that starts with a string - 2 names return - this as well as wrapped in single quotes.  Otherwise only wrapped in single quotes returned
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GenerateValidTableNames(string tableName)
        {
            var rg = new Regex(@"[a-zA-Z][a-zA-Z0-9]*");
            if (rg.IsMatch(tableName))
                yield return tableName;

            yield return string.Format("'{0}'", tableName);
        }

        /// <summary>
        /// Constructs DAX function declarations based on AS server
        /// </summary>
        private void ConstructServerLevelDeclarations()
        {
            List<Declaration> daxFunctionDeclarations = null;
            List<Method> daxMethods = null;

            if (this._daxDocumentProperties != null && !string.IsNullOrEmpty(this._daxDocumentProperties.ConnectionString))
            {
                List<DaxFunction> daxFunctions = ExtractDaxFunctions();
                if (daxFunctions != null)
                {
                    daxFunctionDeclarations = new List<Declaration>();
                    daxMethods = new List<Method>();
                    foreach (DaxFunction daxFunction in daxFunctions)
                    {
                        daxFunctionDeclarations.Add(new Declaration(daxFunction.Description, daxFunction.Name, Babel.LanguageService.FUNCTION_GLYPH_INDEX, daxFunction.Name));

                        // Create a new DAX Method
                        Method daxMethod = new Method();
                        daxMethod.Description = daxFunction.Description;
                        daxMethod.Name = daxFunction.Name;
                        daxMethod.Type = daxFunction.ReturnType.ToString();

                        // Create parameters
                        List<Parameter> daxMethodParams = new List<Parameter>();
                        foreach (DaxFunctionParam daxFunctionParam in daxFunction.ParameterCollection)
                        {
                            Parameter daxMethodParam = new Parameter();
                            daxMethodParam.Description = daxFunctionParam.Type.ToString();
                            daxMethodParam.Name = daxFunctionParam.Name;

                            daxMethodParam.Display = daxFunctionParam.Name;
                            if (daxFunctionParam.IsRepeatable)
                                daxMethodParam.Display += ", ...";
                            if (daxFunctionParam.IsOptional)
                                daxMethodParam.Display = "[" + daxMethodParam.Display + "]";

                            daxMethodParams.Add(daxMethodParam);
                        }
                        daxMethod.Parameters = daxMethodParams;

                        daxMethods.Add(daxMethod);
                    }
                }
            }

            // Commit the newly created declarations into the cache
            lock (this)
            {
                this._daxFunctionDeclarations = daxFunctionDeclarations;
                this._daxMethods = daxMethods;
            }
        }

        #region DAX Functions extraction
        /// <summary>
        /// Extracts the list of DAX functions
        /// </summary>
        private List<DaxFunction> ExtractDaxFunctions()
        {
            // delete existing data
            List<DaxFunction> daxFunctions = null;

            try
            {
                LogLine("Start reading DAX functions.");

                // create a new list of functions to be loaded
                daxFunctions = new List<DaxFunction>();

                // extract scalar functions
                adomd.AdomdRestrictionCollection restrictions = new adomd.AdomdRestrictionCollection();
                restrictions.Add(FUNCTIONS_RESTRICTION_ORIGIN, FUNCTIONS_ORIGIN_SCALAR);
                daxFunctions.AddRange(
                    ExtractDaxFunctionsWithRestrictions(restrictions)
                );

                // extract relational functions
                restrictions.Clear();
                restrictions.Add(FUNCTIONS_RESTRICTION_ORIGIN, FUNCTIONS_ORIGIN_RELATIONAL);
                daxFunctions.AddRange(
                    ExtractDaxFunctionsWithRestrictions(restrictions)
                );

                _completionDataSnapshot = new CompletionDataSnapshot(_completionDataSnapshot, daxFunctions);

                LogLine("End reading DAX functions.");
            }
            catch (Microsoft.AnalysisServices.AdomdClient.AdomdConnectionException)
            {
                // If for some reason connection to the server cannot be established return null, it will be handled outside of this class
                LogLine("Error reading DAX functions.");
            }

            return daxFunctions;
        }

        /// <summary>
        /// Extracts the list of DAX functions with specific restrictions.
        /// </summary>
        /// <param name="restrictions">Restrictions to be applied.</param>
        private List<DaxFunction> ExtractDaxFunctionsWithRestrictions(adomd.AdomdRestrictionCollection restrictions)
        {
            List<DaxFunction> daxFunctions = new List<DaxFunction>();

            DataSet functionData = null;

            if (_adomdConn == null || _adomdConn.State != ConnectionState.Open)
                return daxFunctions;

            try
            {
                functionData = _adomdConn.GetSchemaDataSet(FUNCTIONS_SCHEMA, restrictions);

                // Expecting to receive two tables (one with the functions rowset another with parameter info rows)
                Debug.Assert(functionData.Tables.Count == 2, "Expected MDSCHEMA_FUNCTION to return 2 tables.");

                // Expecting that the tables are related
                Debug.Assert(functionData.Relations.Count > 0, "Expected a relationship in the MDSCHEMA_FUNCTION return dataset.");

                DataRelation parameterInfoRelation = functionData.Relations[FUNCTIONS_RELATION_PARAMETERINFO];
                DataTable functionTable = functionData.Tables[0];

                Debug.Assert(parameterInfoRelation != null, "Cannot find appropriate relation in MDSCHEMA_FUNCTION result.");
                foreach (DataRow row in functionTable.Rows)
                {
                    try
                    {
                        daxFunctions.Add(LoadFunctionDataRow(parameterInfoRelation, row));
                    }
                    catch
                    {
                        // We are ignoring any errors that occur while loading a particular function,
                        // so the rest may be loaded correctly.
                        Debug.Fail("Failed to load function description.");
                    }
                }
            }
            finally
            {
                // Ensure we dispose the DataSet
                if (functionData != null)
                {
                    functionData.Dispose();
                    functionData = null;
                }
            }

            return daxFunctions;
        }


        /// <summary>
        /// Loads a single function from a DataRow of the MDSCHEMA_FUNCTIONS rowset. 
        /// </summary>
        /// <param name="parameterInfoRelation">Relationship to follow to get to parameters
        /// info.</param>
        /// <param name="row">DataRow to load data from</param>
        private DaxFunction LoadFunctionDataRow(DataRelation parameterInfoRelation, DataRow row)
        {
            string functionName = row[FUNCTIONS_FIELD_NAME] as string;
            string functionDescription = row[FUNCTIONS_FIELD_DESCRIPTION] as string;
            FunctionCategory functionCategory = DaxFunction.ConvertFunctionCategory(row[FUNCTIONS_FIELD_CATEGORY] as string);

            // Function name cannot be empty.
            Debug.Assert(!string.IsNullOrEmpty(functionName), "Function name cannot be empty.");

            List<DaxFunctionParam> parameters = new List<DaxFunctionParam>();
            DataRow[] parameterRows = row.GetChildRows(parameterInfoRelation);
            foreach (DataRow parameterRow in parameterRows)
            {
                string name = (string)parameterRow[PARAMETERS_FIELD_NAME];
                bool optional = (bool)parameterRow[PARAMETERS_FIELD_OPTIONAL];
                bool repeatable = (bool)parameterRow[PARAMETERS_FIELD_REPEATABLE];

                DaxFunctionParam parameter = new DaxFunctionParam(name, FormulaType.Any, repeatable, optional);
                parameters.Add(parameter);
            }

            DaxFunction function = new DaxFunction(functionName, functionDescription, functionCategory, FormulaType.Any, parameters);

            return function;
        }
        #endregion

        #endregion Protected methods

        internal void Connect()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackgroundWorker_Connect);
            bw.RunWorkerAsync();
        }

        internal void Disconnect()
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackgroundWorker_Disconnect);
            bw.RunWorkerAsync();
        }

        internal void ExecuteQuery(string queryToExecute)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackgroundWorker_ExecuteQuery);
            bw.RunWorkerAsync(queryToExecute);
        }

        internal void SaveMeasuresAndCalcColumns(string viewText)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(BackgroundWorker_SaveMeasuresAndCalcColumns);
            bw.RunWorkerAsync(viewText);
        }

        internal string GetMeasuresText()
        {
            if (this._database == null)
            {
                if (_isConnecting)
                    return "-- Reading metadata from the database.  Please retry operation later.";

                return "-- Not connected to the database.";
            }

            var sb = new StringBuilder();
            var restrictions = new adomd.AdomdRestrictionCollection();
            restrictions.Add("DatabaseID", _database.ID);
            var dataSet = _adomdConn.GetSchemaDataSet("DISCOVER_XML_METADATA", restrictions);
            Debug.Assert(dataSet.Tables.Count == 1);
            var table = dataSet.Tables[0];
            Debug.Assert(table.Columns.Count == 1);
            Debug.Assert(table.Rows.Count == 1);
            var script = table.Rows[0][0] as string;
            Debug.Assert(!string.IsNullOrEmpty(script));

            var mc = MeasuresContainer.ParseText(script);
            sb.Append(mc.GetDaxText());

            return sb.ToString();
        }

        void BackgroundWorker_Connect(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            bw.DoWork -= new DoWorkEventHandler(BackgroundWorker_Connect);
            _isConnecting = true;
            DoDisconnect();
            ClearDeclarationsCache();

            LogLine("Connecting to...");
            string connectionString = string.Format("Provider=MSOLAP;Application Name=DAX Editor;{0}", this._daxDocumentProperties.ConnectionString);
            LogLine(connectionString);

            try
            {
                DoConnect(connectionString);
                LogLine("Connection succesfull.");
                RefreshDeclarationsCache();
            }
            catch (Exception ex)
            {
                LogError("Error while connecting:" + ex.ToString());
            }
            _isConnecting = false;
        }

        void BackgroundWorker_Disconnect(object sender, DoWorkEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            bw.DoWork -= new DoWorkEventHandler(BackgroundWorker_Disconnect);

            LogLine("Begin disconnecting.");

            DoDisconnect();
            ClearDeclarationsCache();

            LogLine("End disconnecting.");
        }

        void BackgroundWorker_ExecuteQuery(object sender, DoWorkEventArgs e)
        {
            using (DataTable resultDataTable = new DataTable())
            {
                bool isXmla = false;
                for (int iTry = 0; iTry < 2; iTry++)
                {
                    try
                    {
                        var bw = sender as BackgroundWorker;
                        bw.DoWork -= new DoWorkEventHandler(BackgroundWorker_ExecuteQuery);

                        LogLine("Begin executing query.");

                        string commandText = e.Argument as string;
                        // Check first not whitespace char.  If it is ''< assume that the text is XMLA, otherwise it is DAX
                        char firstNonWhitespaceChar = commandText.ToCharArray().AsEnumerable<char>().FirstOrDefault(i => !char.IsWhiteSpace(i));
                        if (firstNonWhitespaceChar == '<')
                            isXmla = true;

                        if (isXmla)
                        {
                            adomd.AdomdConnection adomdClient = null;
                            //XmlaClient xmlaClient = null;
                            try
                            {
                                adomdClient = new adomd.AdomdConnection(this._daxDocumentProperties.ConnectionStringWithoutDatabaseName);
                                adomdClient.Open();
                                var cmd = new adomd.AdomdCommand("@CommandText",adomdClient);
                                cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", commandText));
                                System.Xml.XmlReader reader = cmd.ExecuteXmlReader();
                                var formattedXmlaResult  = reader.ReadOuterXml().ToString();
                                reader.Close();
                                
                                //xmlaClient = new Microsoft.AnalysisServices.XmlaClient();
                                //xmlaClient.Connect(this._daxDocumentProperties.ConnectionStringWithoutDatabaseName);
                                //var result = xmlaClient.Send(commandText, null);
                                //var formattedXmlaResult = XDocument.Parse(result.ToString()).ToString();

                                resultDataTable.Columns.Add("Result");
                                resultDataTable.Rows.Add("See XMLA Result tab");
                                _updateEditorMargin.UpdateResult(resultDataTable);
                                _updateEditorMargin.UpdateXmlaResult(formattedXmlaResult);
                            }
                            finally
                            {
                                if (adomdClient != null)
                                    adomdClient.Close();
                            }
                        }
                        else
                        {
                            using (var cmd = new adomd.AdomdCommand("@CommandText", _adomdConn))
                            {
                                cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", commandText));
                                using (adomd.AdomdDataAdapter dataAdapter = new adomd.AdomdDataAdapter(cmd))
                                {
                                    dataAdapter.Fill(resultDataTable);
                                }
                            }

                            NormalizeHeaders(resultDataTable);
                            _updateEditorMargin.UpdateResult(resultDataTable);
                            _updateEditorMargin.UpdateXmlaResult("See Result tab");
                        }
                        LogLine("End reading response.");

                        LogLine("End executing query.");
                    }
                    catch (adomd.AdomdConnectionException)
                    {
                        if (_adomdConn != null)
                        {
                            LogLine("Connection error while executing the query.  Reconnecting...");
                            var connectionString = _adomdConn.ConnectionString;
                            DoDisconnect();
                            DoConnect(connectionString);
                            continue;
                        }
                    }
                    catch (adomd.AdomdErrorResponseException ex)
                    {
                        LogLine("Error while reading response.");

                        resultDataTable.Columns.Add("Error");
                        resultDataTable.Rows.Add(ex.Message);
                        resultDataTable.Rows[0].RowError = ex.Message;

                        _updateEditorMargin.UpdateResult(resultDataTable);
                        _updateEditorMargin.UpdateXmlaResult(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        LogError("Error while executing query:" + ex.ToString());
                    }
                    // As soon as reached this point do not re-try
                    break;
                }
            }
        }

        void BackgroundWorker_SaveMeasuresAndCalcColumns(object sender, DoWorkEventArgs e)
        {
            var cmdProducer = new ServerCommandProducer(_database.Name, _database.CompatibilityLevel, _database.Cubes[0].Name);

            try
            {
                var bw = sender as BackgroundWorker;
                bw.DoWork -= new DoWorkEventHandler(BackgroundWorker_SaveMeasuresAndCalcColumns);

                LogLine("Begin saving measures.");

                string viewText = e.Argument as string;
                var mc = MeasuresContainer.ParseDaxScript(viewText);

                LogLine("Begin transaction.");
                using (var cmd = new adomd.AdomdCommand("@CommandText", _adomdConn))
                {
                    cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", cmdProducer.ProduceBeginTransaction()));
                    cmd.ExecuteNonQuery();
                }

                LogLine("Alter MDX script.");
                using (var cmd = new adomd.AdomdCommand("@CommandText", _adomdConn))
                {
                    cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", cmdProducer.ProduceAlterMdxScript(mc.Measures)));
                    cmd.ExecuteNonQuery();
                }

                LogLine("Run ProcessRecalc.");
                using (var cmd = new adomd.AdomdCommand("@CommandText", _adomdConn))
                {
                    cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", cmdProducer.ProduceProcessRecalc()));
                    cmd.ExecuteNonQuery();
                }


                LogLine("Commit transaction.");
                using (var cmd = new adomd.AdomdCommand("@CommandText", _adomdConn))
                {
                    cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", cmdProducer.ProduceCommitTransaction()));
                    cmd.ExecuteNonQuery();
                }

                LogLine("End saving measures.");
            }
            catch (Exception ex)
            {
                using (var cmd = new adomd.AdomdCommand("@CommandText", _adomdConn))
                {
                    cmd.Parameters.Add(new adomd.AdomdParameter("CommandText", cmdProducer.ProduceRollbackTransaction()));
                    cmd.ExecuteNonQuery();
                }

                LogError("Error while saving measures. " + ex.ToString());
            }
        }

        private void NormalizeHeaders(DataTable resultDataTable)
        {
            foreach (DataColumn column in resultDataTable.Columns)
            {
                var columnName = column.ColumnName;
                if (columnName.Contains('[') && columnName.Contains(']'))
                {
                    var indexOpeningBrace = columnName.IndexOf('[');
                    var indexClosingBrace = columnName.LastIndexOf(']');
                    Debug.Assert(indexOpeningBrace < indexClosingBrace);
                    column.ColumnName = columnName.Substring(indexOpeningBrace + 1, indexClosingBrace - indexOpeningBrace - 1);
                }

            }
        }

        private void DoConnect(string connectionString)
        {
            _adomdConn = new adomd.AdomdConnection(connectionString);
            _adomdConn.Open();
        }

        private void DoDisconnect()
        {
            if (_adomdConn != null)
            {
                _adomdConn.Close();
                _adomdConn = null;
            }

            if (_database != null)
            {
                _database.Dispose();
                _database = null;
            }
        }

        public void SetUpdateEditorMargin(IUpdateEditorMargin updateDaxQueryResult)
        {
            _updateEditorMargin = updateDaxQueryResult;
        }

        public static string HtmlEncode(string input)
        {
            var result = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                switch (ch)
                {
                    case '<':
                        result.Append("&lt;");
                        break;
                    case '>':
                        result.Append("&gt;");
                        break;
                    case '"':
                        result.Append("&quot;");
                        break;
                    case '&':
                        result.Append("&amp;");
                        break;
                    case '\'':
                        result.Append("&#39;");
                        break;
                    default:
                        result.Append(ch);
                        break;
                }
            }

            return result.ToString();
        }

        public CompletionDataSnapshot GetCompletionDataSnapshot()
        {
            return _completionDataSnapshot;
        }
    }
}
