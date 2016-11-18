// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaxEditor
{
    /// <summary>
    /// Enumeration of types supported in DAX for both parameters of functions and their
    /// return values.
    /// </summary>
    /// <remarks>This has the flags attribute because parameter types may allow more than one
    /// type. Also, function returns may be indeterminate in the same way.</remarks>
    [Flags]
    public enum FormulaType
    {
        Unknown = 0,    // Must be 0.
        I8 = 1,
        R8 = 2,
        String = 4,
        Bool = 8,
        Date = 16,
        Currency = 32,
        Any = 64,
        Numeric = 128,
        None = 256,
        Scalar = 512,
        ColumnReference = 1024,
        TableReference = 2048,
        Table = 4096
    };

    /// <summary>
    // Enumeration of the category a function belongs to
    /// </summary>
    public enum FunctionCategory
    {
        None,           // only used when some kind of error prevents us from finding the real category, or it's missing
        DateAndTime,    // DATETIME
        Information,    // INFO
        Logical,        // LOGICAL
        MathAndTrig,    // MATHTRIG
        Statistical,    // STATISTICAL
        Text,           // TEXT
        Deprecated      // DEPRECATED
    };

    public abstract class BaseFunction
    {
        /// <summary>
        /// Function name
        /// </summary>
        private string fnName;
        public string Name
        {
            set { fnName = value; }
            get { return fnName; }
        }

        /// <summary>
        /// Function description
        /// </summary>
        private string fnDescription;
        public string Description
        {
            set { fnDescription = value == null ? string.Empty : value; }
            get { return fnDescription; }
        }

        /// <summary>
        /// Function category
        /// </summary>
        private FunctionCategory fnCategory;
        public FunctionCategory Category
        {
            get { return fnCategory; }
            set
            {
                if (value != FunctionCategory.None)
                {
                    fnCategory = value;
                }
            }
        }
    }

    public class DaxFunction : BaseFunction
    {
        #region Static fields related to Function Categories
        /// <summary>
        /// Builds a Dictionary that converts engine category strings into values of FunctionCategory
        /// corresponding them.
        /// </summary>
        /// <returns>A Dictionary that converts engine category strings into values of FunctionCategory.
        /// </returns>
        private static Dictionary<string, FunctionCategory> LoadFunctionCategoriesConversionDictionary()
        {
            // These values are currently based on the macros in:
            // Engine/src/md/internalsp/mdinternalxl.cpp
            Dictionary<string, FunctionCategory> dictionary = new Dictionary<string, FunctionCategory>(7);
            dictionary.Add("DATETIME", FunctionCategory.DateAndTime);
            dictionary.Add("DEPRECATED", FunctionCategory.Deprecated);
            dictionary.Add("INFO", FunctionCategory.Information);
            dictionary.Add("LOGICAL", FunctionCategory.Logical);
            dictionary.Add("MATHTRIG", FunctionCategory.MathAndTrig);
            dictionary.Add("STATISTICAL", FunctionCategory.Statistical);
            dictionary.Add("TEXT", FunctionCategory.Text);

            return dictionary;
        }

        public static readonly Dictionary<string, FunctionCategory> functionCategories = LoadFunctionCategoriesConversionDictionary();


        /// <summary>
        /// Converts an engine string representing a function category into a FunctionCategory 
        /// enum value.
        /// </summary>
        /// <param name="engineCategory">String received from the engine representing the category.</param>
        /// <returns>The appropriate FunctionCategory value for the string, or, if one cannot be
        /// found, FunctionCategory.None.</returns>
        public static FunctionCategory ConvertFunctionCategory(string engineCategory)
        {
            if (string.IsNullOrEmpty(engineCategory))
            {
                return FunctionCategory.None;
            }

            FunctionCategory category;
            if (functionCategories.TryGetValue(engineCategory, out category))
            {
                return category;
            }
            else
            {
                return FunctionCategory.None;
            }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a DaxFunction object with default values
        /// </summary>
        internal DaxFunction()
        {
            //this.repeatStart         = 0;
            this.fnParameters = new List<DaxFunctionParam>();
            this.fnReturnType = FormulaType.None;
            this.Description = string.Empty;
            this.Category = FunctionCategory.None;
        }

        /// <summary>
        /// Initialize a DaxFunction object with specific values
        /// </summary>
        /// <param name="name">The function name</param>
        internal DaxFunction(string name)
        {
            this.Name = name;
            this.ReturnType = FormulaType.None;
            this.ParameterCollection = null;
            this.Description = string.Empty;
            this.Category = FunctionCategory.None;

        }

        /// <summary>
        /// Initialize a DAXFunction object with specific values
        /// </summary>
        /// <param name="name">The function name</param>
        /// <param name="description">The function description</param>
        /// <param name="category">The category the function belongs to</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="functionParameters">A list of function parameters</param>
        internal DaxFunction(string name, string description, FunctionCategory category,
                            FormulaType returnType, List<DaxFunctionParam> functionParameters)
        {
            this.Name = name;
            this.ReturnType = returnType;
            this.ParameterCollection = functionParameters;
            this.Description = description;
            this.Category = category;
        }

        /// <summary>
        /// Initialize a DAXFunction object with specific values
        /// </summary>
        /// <param name="name">The function name</param>
        /// <param name="description">The function description</param>
        /// <param name="category">The category the function belongs to</param>
        /// <param name="returnType">The return type of the function.</param>
        /// <param name="functionParameters">An array of function parameters</param>
        internal DaxFunction(string name, string description, FunctionCategory category,
                            FormulaType returnType, DaxFunctionParam[] functionParameters)
        {
            this.Name = name;
            this.ReturnType = returnType;
            this.ParameterCollection = functionParameters == null ? null
                                                                  : new List<DaxFunctionParam>(functionParameters);
            this.Description = description;
            this.Category = category;

        }
        #endregion

        #region Members Name/ReturnType/ParamInfo

        /// <summary>
        /// Function return type
        /// </summary>
        private FormulaType fnReturnType;
        public FormulaType ReturnType
        {
            set { fnReturnType = value; }
            get { return fnReturnType; }
        }

        /// <summary>
        /// Function parameters list
        /// </summary>
        private List<DaxFunctionParam> fnParameters;
        public List<DaxFunctionParam> ParameterCollection
        {
            set { fnParameters = value; }
            get { return fnParameters; }
        }

        /// <summary>
        /// Prototype of the function
        /// </summary>
        public string Prototype
        {
            get
            {
                StringBuilder sbPrototype = new StringBuilder();
                StringBuilder sbParams = new StringBuilder();

                string repeatIndicator = "...";
                char separatorSymbol = ',';
                bool separatorNeeded = false;
                foreach (DaxFunctionParam param in this.ParameterCollection)
                {
                    if (separatorNeeded)
                        sbParams.Append(separatorSymbol);

                    if (param.IsOptional)
                        sbParams.AppendFormat("[{0}]", param.Name);
                    else
                        sbParams.AppendFormat("{0}", param.Name);

                    separatorNeeded = true;

                    if (param.IsRepeatable)
                    {
                        sbParams.Append(separatorSymbol);
                        sbParams.AppendFormat("[{0}]", param.Name);
                        sbParams.Append(separatorSymbol);
                        sbParams.Append(repeatIndicator);
                    }
                }

                sbPrototype.AppendFormat("{0}({1})", this.Name, sbParams.ToString());

                return sbPrototype.ToString();
            }
        }
        #endregion
    }


    public class DaxFunctionParam
    {
        // Our members
        protected string paramName;
        protected FormulaType paramType;
        protected bool repeatable;
        protected bool optional;

        /// <summary>
        /// Initialize a DAXFunctionParam object with default values
        /// </summary>
        internal DaxFunctionParam()
        {
            this.paramName = string.Empty;
            this.paramType = FormulaType.None;
            this.repeatable = false;
            this.optional = false;
        }

        /// <summary>
        /// Initialize a DAXFunctionParam object with a name and type
        /// </summary>
        /// <param name="name">The parameter's name</param>
        /// <param name="type">The parameters type</param>
        internal DaxFunctionParam(string name, FormulaType type)
        {
            this.paramName = name;
            this.paramType = type;
            this.repeatable = false;
            this.optional = false;
        }

        /// <summary>
        /// Initialize a DAXFunctionParam object with specific values
        /// </summary>
        /// <param name="name">The parameter's name</param>
        /// <param name="type">The parameters type</param>
        /// <param name="repeatable">Whether or not the parameter can be repeated</param>
        /// <param name="optional">Whether or not the parameter is optional</param>
        internal DaxFunctionParam(string name, FormulaType type, bool repeatable, bool optional)
        {
            this.paramName = name;
            this.paramType = type;
            this.repeatable = repeatable;
            this.optional = optional;
        }

        /// <summary>
        /// Get/Set the parameter's name
        /// </summary>
        /// <returns>The parameter's name</returns>
        internal string Name
        {
            get
            {
                return this.paramName;
            }

            set
            {
                this.paramName = value;
            }
        }

        /// <summary>
        /// Get/Set the parameter's type
        /// </summary>
        /// <returns>The parameter's type</returns>
        internal FormulaType Type
        {
            get
            {
                return this.paramType;
            }

            set
            {
                this.paramType = value;
            }
        }

        /// <summary>
        /// Is this parameter repeatable
        /// </summary>
        /// <returns>true if repeatable, false if not</returns>
        internal bool IsRepeatable
        {
            get
            {
                return this.repeatable;
            }

            set
            {
                this.repeatable = value;
            }
        }

        /// <summary>
        /// Is this parameter optional
        /// </summary>
        /// <returns>true if optional, false if not</returns>
        internal bool IsOptional
        {
            get
            {
                return this.optional;
            }

            set
            {
                this.optional = value;
            }
        }

        /// <summary>
        /// Get the parameter name as it should appear in the prototype string
        /// </summary>
        /// <param name="iRepeat">The repeat value for a repeatable parameter, ignored for non repeating params</param>
        /// <returns>The parameter prototype name</returns>
        internal string GetNameForPrototype(int iRepeat)
        {
            StringBuilder name = new StringBuilder();

            // Are we repeatable
            if (this.repeatable)
            {
                // Yes, inc iRepeat since it is zero based but the appended
                // numbers should be one based
                iRepeat++;

                // Are we optional or doing any but the first repeat
                if (this.optional || iRepeat > 1)
                {
                    // Yes, the parameter is surrounded by the optional indicator
                    // and has a number appended to it
                    name.AppendFormat("[{0}{1}]", this.paramName, iRepeat);
                }
                else
                {
                    // No, the name just has a number appended to it
                    name.AppendFormat("{0}{1}", this.paramName, iRepeat);
                }
            }
            else
            {
                // No, are we optional
                if (this.optional)
                {
                    // Yes, the parameter is surrounded by the optional indicator
                    name.AppendFormat("[{0}]", this.paramName);
                }
                else
                {
                    // No, the name is just the name (profound isn't it)
                    name.Append(this.paramName);
                }
            }
            return name.ToString();
        }
    }
}
