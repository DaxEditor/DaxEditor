// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaxEditor.GeneratorSource
{
    public class DaxFunctions : List<DaxFunctionDescription>
    {
        private static TernaryTree<string> _tree = null;
        private static object SyncObject = new object();

        public static TernaryTree<string> GetFunctionsTree()
        {
            if(_tree == null)
            {
                lock(SyncObject)
                {
                    if(_tree == null)
                    {
                        _tree = new TernaryTree<string>();
                        var functions = new DaxFunctions();
                        foreach(var func in functions)
                        {
                            _tree.AddWord(func.Name, func.Name);
                        }
                        _tree.PrepareForSearch();
                    }
                }
            }
            return _tree;
        }


        public DaxFunctions()
        {
            List<Babel.Parameter> parameters = null;
            Babel.Parameter parameter;

//
// Beggining of generated code

      // DATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Year";
      parameter.Display = @"Year";
      parameter.Description = @"A four digit number representing the year.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Month";
      parameter.Display = @"Month";
      parameter.Description = @"A number from 1 to 12 representing the month of the year.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Day";
      parameter.Display = @"Day";
      parameter.Description = @"A number from 1 to 31 representing the day of the month.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATE", Description = @"Returns the specified date in datetime format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATEDIFF
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Date1";
      parameter.Display = @"Date1";
      parameter.Description = @"A date in datetime format that represents the start date.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Date2";
      parameter.Display = @"Date2";
      parameter.Description = @"A date in datetime format that represents the end date.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Interval";
      parameter.Display = @"Interval";
      parameter.Description = @"The unit that will be used to calculate, between the two dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATEDIFF", Description = @"Returns the number of units (unit specified in Interval) between the input two dates.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATEVALUE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"DateText";
      parameter.Display = @"DateText";
      parameter.Description = @"A text string that represents a date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATEVALUE", Description = @"Converts a date in the form of text to a date in datetime format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DAY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Date";
      parameter.Display = @"Date";
      parameter.Description = @"A date in datetime format.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DAY", Description = @"Returns a number from 1 to 31 representing the day of the month.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // EDATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"StartDate";
      parameter.Display = @"StartDate";
      parameter.Description = @"A date in datetime format that represents the start date.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Months";
      parameter.Display = @"Months";
      parameter.Description = @"The number of months before or after start_date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EDATE", Description = @"Returns the date that is the indicated number of months before or after the start date.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // EOMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"StartDate";
      parameter.Display = @"StartDate";
      parameter.Description = @"The start date in datetime format.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Months";
      parameter.Display = @"Months";
      parameter.Description = @"The number of months before or after the start_date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EOMONTH", Description = @"Returns the date in datetime format of the last day of the month before or after a specified number of months.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // HOUR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Datetime";
      parameter.Display = @"Datetime";
      parameter.Description = @"A datetime value or text in time format, such as 16:48:00 or 4:48:00 PM.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"HOUR", Description = @"Returns the hour as a number from 0 (12:00 A.M.) to 23 (11:00 P.M.).", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // MINUTE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Datetime";
      parameter.Display = @"Datetime";
      parameter.Description = @"A datetime value or text in time format, such as 16:48:00 or 4:48:00 PM.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MINUTE", Description = @"Returns a number from 0 to 59 representing the minute.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // MONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Date";
      parameter.Display = @"Date";
      parameter.Description = @"A date in datetime format.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MONTH", Description = @"Returns a number from 1 (January) to 12 (December) representing the month.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // NOW
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"NOW", Description = @"Returns the current date and time in datetime format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // SECOND
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Datetime";
      parameter.Display = @"Datetime";
      parameter.Description = @"The time in datetime format or text in time format, such as 16:48:23 or 4:48:47 PM.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SECOND", Description = @"Returns a number from 0 to 59 representing the second.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // TIME
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Hour";
      parameter.Display = @"Hour";
      parameter.Description = @"A number from 0 to 23 representing the hour.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Minute";
      parameter.Display = @"Minute";
      parameter.Description = @"A number from 0 to 59 representing the minute.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Second";
      parameter.Display = @"Second";
      parameter.Description = @"A number from 0 to 59 representing the second.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TIME", Description = @"Converts hours, minutes, and seconds given as numbers to a time in datetime format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // TIMEVALUE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TimeText";
      parameter.Display = @"TimeText";
      parameter.Description = @"A text string that gives a time; date information in the string is ignored.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TIMEVALUE", Description = @"Converts a time in text format to a time in datetime format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // TODAY
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"TODAY", Description = @"Returns the current date in datetime format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // WEEKDAY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Date";
      parameter.Display = @"Date";
      parameter.Description = @"A date in datetime format.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ReturnType";
      parameter.Display = @"ReturnType";
      parameter.Description = @"A number that determines the return value: for Sunday=1 through Saturday=7, use 1; for Monday=1 through Sunday=7, use 2; for Monday=0 through Sunday=6, use 3.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"WEEKDAY", Description = @"Returns a number from 1 to 7 identifying the day of the week of a date.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // WEEKNUM
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Date";
      parameter.Display = @"Date";
      parameter.Description = @"A date in datetime format.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ReturnType";
      parameter.Display = @"ReturnType";
      parameter.Description = @"A number that determines the return value: use 1 when week begins on Sunday, or use 2 when week begins on Monday.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"WEEKNUM", Description = @"Returns the week number in the year.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // YEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Date";
      parameter.Display = @"Date";
      parameter.Description = @"A date in datetime format.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"YEAR", Description = @"Returns the year of a date as a four digit integer.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // YEARFRAC
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"StartDate";
      parameter.Display = @"StartDate";
      parameter.Description = @"The start date in datetime format.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"EndDate";
      parameter.Display = @"EndDate";
      parameter.Description = @"The end date in datetime format.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Basis";
      parameter.Display = @"Basis";
      parameter.Description = @"The type of day count basis to use.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"YEARFRAC", Description = @"Returns the year fraction representing the number of whole days between start_date and end_date.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // CONTAINS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table to test.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column in the input table or in a related table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"A scalar expression to look for in the column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CONTAINS", Description = @"Returns TRUE if there exists at least one row where all columns have specified values.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // CUSTOMDATA
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"CUSTOMDATA", Description = @"Returns the value of the CustomData connection string property if defined; otherwise, BLANK().", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // HASONEFILTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column to check the filter info.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"HASONEFILTER", Description = @"Returns true the specified table or column have one and only one filter.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // HASONEVALUE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column to check the filter info.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"HASONEVALUE", Description = @"Returns true when there's only one value in the specified column.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISBLANK
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISBLANK", Description = @"Checks whether a value is blank, and returns TRUE or FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISCROSSFILTERED
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TableNameOrColumnName";
      parameter.Display = @"TableNameOrColumnName";
      parameter.Description = @"The column or table to check the filter info.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISCROSSFILTERED", Description = @"Returns true when the specified table or column is crossfiltered.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISEMPTY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table or table-expression.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISEMPTY", Description = @"Returns true if the specified table or table-expression is Empty.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISERROR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISERROR", Description = @"Checks whether a value is an error, and returns TRUE or FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISEVEN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value to test. If number is not an integer, it is truncated.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISEVEN", Description = @"Returns TRUE if number is even, or FALSE if number is odd.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISFILTERED
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column to check the filter info.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISFILTERED", Description = @"Returns true when there are direct filters on the specified column.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISLOGICAL
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISLOGICAL", Description = @"Checks whether a value is a logical value (TRUE or FALSE), and returns TRUE or FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISNONTEXT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISNONTEXT", Description = @"Checks whether a value is not text (blank cells are not text), and returns TRUE or FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISNUMBER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISNUMBER", Description = @"Checks whether a value is a number, and returns TRUE or FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISODD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value to test. If number is not an integer, it is truncated.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISODD", Description = @"Returns TRUE if number is odd, or FALSE if number is even.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISSUBTOTAL
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISSUBTOTAL", Description = @"Returns TRUE if the current row contains a subtotal for a specified column and FALSE otherwise.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // ISTEXT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISTEXT", Description = @"Checks whether a value is text, and returns TRUE or FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // USERNAME
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"USERNAME", Description = @"Returns the domain name and user name of the current connection with the format of domain-name\user-name.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Info, Parameters = parameters });

      // AND
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Logical1";
      parameter.Display = @"Logical1";
      parameter.Description = @"The logical values you want to test.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Logical2";
      parameter.Display = @"Logical2";
      parameter.Description = @"The logical values you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"AND", Description = @"Checks whether all arguments are TRUE, and returns TRUE if all arguments are TRUE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // FALSE
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"FALSE", Description = @"Returns the logical value FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // IF
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"LogicalTest";
      parameter.Display = @"LogicalTest";
      parameter.Description = @"Any value or expression that can be evaluated to TRUE or FALSE.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ResultIfTrue";
      parameter.Display = @"ResultIfTrue";
      parameter.Description = @"The value that is returned if the logical test is TRUE; if omitted, TRUE is returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ResultIfFalse";
      parameter.Display = @"ResultIfFalse";
      parameter.Description = @"The value that is returned if the logical test is FALSE; if omitted, FALSE is returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"IF", Description = @"Checks whether a condition is met, and returns one value if TRUE, and another value if FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // IFERROR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"Any value or expression.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ValueIfError";
      parameter.Display = @"ValueIfError";
      parameter.Description = @"Any value or expression.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"IFERROR", Description = @"Returns value_if_error if the first expression is an error and the value of the expression itself otherwise.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // NOT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Logical";
      parameter.Display = @"Logical";
      parameter.Description = @"A value or expression that can be evaluated to TRUE or FALSE.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NOT", Description = @"Changes FALSE to TRUE, or TRUE to FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // OR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Logical1";
      parameter.Display = @"Logical1";
      parameter.Description = @"The logical values you want to test.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Logical2";
      parameter.Display = @"Logical2";
      parameter.Description = @"The logical values you want to test.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"OR", Description = @"Returns TRUE if any of the arguments are TRUE, and returns FALSE if all arguments are FALSE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // SWITCH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"If expression has this value the corresponding result will be returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Result";
      parameter.Display = @"Result";
      parameter.Description = @"The result to be returned if Expression has corresponding value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Else";
      parameter.Display = @"Else";
      parameter.Description = @"If there are no matching values the Else value is returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SWITCH", Description = @"Returns different results depending on the value of an expression.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // TRUE
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"TRUE", Description = @"Returns the logical value TRUE.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Logical, Parameters = parameters });

      // ABS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number for which you want the absolute value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ABS", Description = @"Returns the absolute value of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ACOS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The cosine of the angle you want and must be from -1 to 1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ACOS", Description = @"Returns the arccosine, or inverse cosine, of a number. The arccosine is the angle whose cosine is number. The returned angle is given in radians in the range 0 (zero) to pi.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ACOSH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number equal to or greater than 1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ACOSH", Description = @"Returns the inverse hyperbolic cosine of a number. The number must be greater than or equal to 1. The inverse hyperbolic cosine is the value whose hyperbolic cosine is number, so ACOSH(COSH(number)) equals number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ACOT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Number is the cotangent of the angle you want. This must be a real number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ACOT", Description = @"Returns the principal value of the arccotangent, or inverse cotangent, of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ACOTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The absolute value of Number must be greater than 1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ACOTH", Description = @"Returns the inverse hyperbolic cotangent of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ASIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The sine of the angle you want and must be from -1 to 1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ASIN", Description = @"Returns the arcsine, or inverse sine, of a number. The arcsine is the angle whose sine is number. The returned angle is given in radians in the range -pi/2 to pi/2.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ASINH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ASINH", Description = @"Returns the inverse hyperbolic sine of a number. The inverse hyperbolic sine is the value whose hyperbolic sine is number, so ASINH(SINH(number)) equals number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ATAN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @" The tangent of the angle you want.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ATAN", Description = @"Returns the arctangent, or inverse tangent, of a number. The arctangent is the angle whose tangent is number. The returned angle is given in radians in the range -pi/2 to pi/2.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ATANH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number between 1 and -1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ATANH", Description = @"Returns the inverse hyperbolic tangent of a number. Number must be between -1 and 1 (excluding -1 and 1). The inverse hyperbolic tangent is the value whose hyperbolic tangent is number, so ATANH(TANH(number)) equals number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // BETA.DIST
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"X";
      parameter.Display = @"X";
      parameter.Description = @"The value between A and B at which to evaluate the function.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Alpha";
      parameter.Display = @"Alpha";
      parameter.Description = @"A parameter of the distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Beta";
      parameter.Display = @"Beta";
      parameter.Description = @"A parameter the distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Cumulative";
      parameter.Display = @"Cumulative";
      parameter.Description = @" A logical value that determines the form of the function. If cumulative is TRUE, BETA.DIST returns the cumulative distribution function; if FALSE, it returns the probability density function.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"A";
      parameter.Display = @"A";
      parameter.Description = @"A lower bound to the interval of x.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"B";
      parameter.Display = @"B";
      parameter.Description = @"An upper bound to the interval of x.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"BETA.DIST", Description = @"Returns the beta distribution. The beta distribution is commonly used to study variation in the percentage of something across samples, such as the fraction of the day people spend watching television.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // BETA.INV
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Probability";
      parameter.Display = @"Probability";
      parameter.Description = @"A probability associated with the beta distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Alpha";
      parameter.Display = @"Alpha";
      parameter.Description = @"A parameter of the distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Beta";
      parameter.Display = @"Beta";
      parameter.Description = @"A parameter the distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"A";
      parameter.Display = @"A";
      parameter.Description = @"A lower bound to the interval of x.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"B";
      parameter.Display = @"B";
      parameter.Description = @"An upper bound to the interval of x.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"BETA.INV", Description = @"Returns the inverse of the beta cumulative probability density function (BETA.DIST). If probability = BETA.DIST(x,...TRUE), then BETA.INV(probability,...) = x. The beta distribution can be used in project planning to model probable completion times given an expected completion time and variability.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CEILING
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value you want to round.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Significance";
      parameter.Display = @"Significance";
      parameter.Description = @"The multiple to which you want to round.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CEILING", Description = @"Rounds a number up, to the nearest integer or to the nearest unit of significance.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CHISQ.DIST
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"X";
      parameter.Display = @"X";
      parameter.Description = @"The value at which you want to evaluate the distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Deg_freedom";
      parameter.Display = @"Deg_freedom";
      parameter.Description = @"The number of degrees of freedom.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Cumulative";
      parameter.Display = @"Cumulative";
      parameter.Description = @"A logical value that determines the form of the function. If cumulative is TRUE, CHISQ.DIST returns the cumulative distribution function; if FALSE, it returns the probability density function.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CHISQ.DIST", Description = @"Returns the chi-squared distribution. The chi-squared distribution is commonly used to study variation in the percentage of something across samples, such as the fraction of the day people spend watching television.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CHISQ.DIST.RT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"X";
      parameter.Display = @"X";
      parameter.Description = @"The value at which you want to evaluate the distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Deg_freedom";
      parameter.Display = @"Deg_freedom";
      parameter.Description = @"The number of degrees of freedom.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CHISQ.DIST.RT", Description = @"Returns the right-tailed probability of the chi-squared distribution. The chi-squared distribution is associated with a chi-squared test. Use the chi-squared test to compare observed and expected values. For example, a genetic experiment might hypothesize that the next generation of plants will exhibit a certain set of colors. By comparing the observed results with the expected ones, you can decide whether your original hypothesis is valid.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CHISQ.INV
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Probability";
      parameter.Display = @"Probability";
      parameter.Description = @"A probability associated with the chi-squared distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Deg_freedom";
      parameter.Display = @"Deg_freedom";
      parameter.Description = @"The number of degrees of freedom.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CHISQ.INV", Description = @"Returns the inverse of the left-tailed probability of the chi-squared distribution. The chi-squared distribution is commonly used to study variation in the percentage of something across samples, such as the fraction of the day people spend watching television.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CHISQ.INV.RT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Probability";
      parameter.Display = @"Probability";
      parameter.Description = @"A probability associated with the chi-squared distribution.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Deg_freedom";
      parameter.Display = @"Deg_freedom";
      parameter.Description = @"The number of degrees of freedom.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CHISQ.INV.RT", Description = @"Returns the inverse of the right-tailed probability of the chi-squared distribution. If probability = CHISQ.DIST.RT(x,...), then CHISQ.INV.RT(probability,...) = x. Use this function to compare observed results with expected ones in order to decide whether your original hypothesis is valid.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // COMBIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number of items.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Number_chosen";
      parameter.Display = @"Number_chosen";
      parameter.Description = @"The number of items in each combination.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COMBIN", Description = @"Returns the number of combinations for a given number of items. Use COMBIN to determine the total possible number of groups for a given number of items.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // COMBINA
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Must be greater than or equal to 0, and greater than or equal to Number_chosen. Non-integer values are truncated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Number_chosen";
      parameter.Display = @"Number_chosen";
      parameter.Description = @"Must be greater than or equal to 0. Non-integer values are truncated.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COMBINA", Description = @"Returns the number of combinations (with repetitions) for a given number of items.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CONFIDENCE.NORM
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Alpha";
      parameter.Display = @"Alpha";
      parameter.Description = @"The significance level used to compute the confidence level. The confidence level equals 100*(1 - alpha)%, or in other words, an alpha of 0.05 indicates a 95 percent confidence level.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Standard_dev";
      parameter.Display = @"Standard_dev";
      parameter.Description = @"The population standard deviation for the data range and is assumed to be known.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Size";
      parameter.Display = @"Size";
      parameter.Description = @"The sample size.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CONFIDENCE.NORM", Description = @"Returns the confidence interval for a population mean, using a normal distribution.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CONFIDENCE.T
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Alpha";
      parameter.Display = @"Alpha";
      parameter.Description = @"The significance level used to compute the confidence level. The confidence level equals 100*(1 - alpha)%, or in other words, an alpha of 0.05 indicates a 95 percent confidence level.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Standard_dev";
      parameter.Display = @"Standard_dev";
      parameter.Description = @"The population standard deviation for the data range and is assumed to be known.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Size";
      parameter.Display = @"Size";
      parameter.Description = @"The sample size.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CONFIDENCE.T", Description = @"Returns the confidence interval for a population mean, using a Student's t distribution.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // COS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The angle in radians for which you want the cosine.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COS", Description = @"Returns the cosine of the given angle.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // COSH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number for which you want to find the hyperbolic cosine.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COSH", Description = @"Returns the hyperbolic cosine of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // COT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The angle in radians for which you want the cotangent.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COT", Description = @"Return the cotangent of an angle specified in radians.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // COTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COTH", Description = @"Return the hyperbolic cotangent of a hyperbolic angle.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // CURRENCY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"A scalar expression to be converted to currency data type.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CURRENCY", Description = @"Returns the value as a currency data type.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // DEGREES
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The angle in radians that you want to convert.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DEGREES", Description = @"Converts radians into degrees.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // EVEN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value to round.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EVEN", Description = @"Returns number rounded up to the nearest even integer. You can use this function for processing items that come in twos. For example, a packing crate accepts rows of one or two items. The crate is full when the number of items, rounded up to the nearest two, matches the crate's capacity.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // EXP
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The exponent that is applied to the base e. The constant e equals 2.71828182845904, the base of the natural logarithm.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EXP", Description = @"Returns e raised to the power of a given number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // EXPON.DIST
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"X";
      parameter.Display = @"X";
      parameter.Description = @"The value of the function.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Lambda";
      parameter.Display = @"Lambda";
      parameter.Description = @"The parameter value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Cumulative";
      parameter.Display = @"Cumulative";
      parameter.Description = @"A logical value that indicates which form of the exponential function to provide. If cumulative is TRUE, EXPON.DIST returns the cumulative distribution function; if FALSE, it returns the probability density function.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EXPON.DIST", Description = @"Returns the exponential distribution. Use EXPON.DIST to model the time between events, such as how long an automated bank teller takes to deliver cash. For example, you can use EXPON.DIST to determine the probability that the process takes at most 1 minute.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // FACT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The nonnegative number you want the factorial of.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FACT", Description = @"Returns the factorial of a number, equal to 1*2*3*...* Number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // FLOOR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number you want to round.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Significance";
      parameter.Display = @"Significance";
      parameter.Description = @"The multiple to which you want to round. Number and significance must either both be positive or both be negative.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FLOOR", Description = @"Rounds a number down, toward zero, to the nearest multiple of significance.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // GCD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number1";
      parameter.Display = @"Number1";
      parameter.Description = @"The first number, if value is not an integer, it is truncated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Number2";
      parameter.Display = @"Number2";
      parameter.Description = @"The second number, if value is not an integer, it is truncated.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"GCD", Description = @"Returns the greatest common divisor of two integers. The greatest common divisor is the largest integer that divides both number1 and number2 without a remainder.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // INT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number you want to round down to an integer.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"INT", Description = @"Rounds a number down to the nearest integer.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ISO.CEILING
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value you want to round.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Significance";
      parameter.Display = @"Significance";
      parameter.Description = @"The multiple to which you want to round.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISO.CEILING", Description = @"Rounds a number up, to the nearest integer or to the nearest multiple of significance.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // LCM
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number1";
      parameter.Display = @"Number1";
      parameter.Description = @"The first number, if value is not an integer, it is truncated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Number2";
      parameter.Display = @"Number2";
      parameter.Description = @"The second number, if value is not an integer, it is truncated.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LCM", Description = @"Returns the least common multiple of integers. The least common multiple is the smallest positive integer that is a multiple of both integer arguments number1, number2. Use LCM to add fractions with different denominators.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // LN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The positive number for which you want the natural logarithm.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LN", Description = @"Returns the natural logarithm of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // LOG
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The positive number for which you want the logarithm.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Base";
      parameter.Display = @"Base";
      parameter.Description = @"The base of the logarithm; if omitted, 10.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LOG", Description = @"Returns the logarithm of a number to the base you specify.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // LOG10
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The positive number for which you want the base-10 logarithm.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LOG10", Description = @"Returns the base-10 logarithm of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // MOD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number for which you want to find the remainder after the division is performed.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Divisor";
      parameter.Display = @"Divisor";
      parameter.Description = @"The number by which you want to divide.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MOD", Description = @"Returns the remainder after a number is divided by a divisor.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // MROUND
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value to round.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Multiple";
      parameter.Display = @"Multiple";
      parameter.Description = @"The multiple to which you want to round the number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MROUND", Description = @"Returns a number rounded to the desired multiple.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ODD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The value to round.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ODD", Description = @"Returns number rounded up to the nearest odd integer.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // PERMUT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"An integer that describes the number of objects.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Number_chosen";
      parameter.Display = @"Number_chosen";
      parameter.Description = @"An integer that describes the number of objects in each permutation.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PERMUT", Description = @"Returns the number of permutations for a given number of objects that can be selected from number objects. A permutation is any set or subset of objects or events where internal order is significant. Permutations are different from combinations, for which the internal order is not significant. Use this function for lottery-style probability calculations.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // PI
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"PI", Description = @"Returns the value of Pi, 3.14159265358979, accurate to 15 digits.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // POISSON.DIST
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"X";
      parameter.Display = @"X";
      parameter.Description = @"The number of events.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Mean";
      parameter.Display = @"Mean";
      parameter.Description = @"The expected numeric value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Cumulative";
      parameter.Display = @"Cumulative";
      parameter.Description = @"A logical value that determines the form of the probability distribution returned. If cumulative is TRUE, POISSON.DIST returns the cumulative Poisson probability that the number of random events occurring will be between zero and x inclusive; if FALSE, it returns the Poisson probability mass function that the number of events occurring will be exactly x.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"POISSON.DIST", Description = @"Returns the Poisson distribution. A common application of the Poisson distribution is predicting the number of events over a specific time, such as the number of cars arriving at a toll plaza in 1 minute.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // POWER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The base number.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Power";
      parameter.Display = @"Power";
      parameter.Description = @"The exponent, to which the base number is raised.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"POWER", Description = @"Returns the result of a number raised to a power.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // QUOTIENT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Numerator";
      parameter.Display = @"Numerator";
      parameter.Description = @"The dividend.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Denominator";
      parameter.Display = @"Denominator";
      parameter.Description = @"The divisor.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"QUOTIENT", Description = @"Returns the integer portion of a division.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // RADIANS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"An angle in degrees that you want to convert.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RADIANS", Description = @"Converts degrees to radians.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // RAND
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"RAND", Description = @"Returns a random number greater than or equal to 0 and less than 1, evenly distributed. Random numbers change on recalculation.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // RANDBETWEEN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Bottom";
      parameter.Display = @"Bottom";
      parameter.Description = @"The smallest integer RANDBETWEEN will return.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Top";
      parameter.Display = @"Top";
      parameter.Description = @"The largest integer RANDBETWEEN will return.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RANDBETWEEN", Description = @"Returns a random number between the numbers you specify.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ROUND
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number you want to round.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfDigits";
      parameter.Display = @"NumberOfDigits";
      parameter.Description = @"The number of digits to which you want to round. Negative rounds to the left of the decimal point; zero to the nearest integer.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROUND", Description = @"Rounds a number to a specified number of digits.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ROUNDDOWN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number that you want rounded down.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfDigits";
      parameter.Display = @"NumberOfDigits";
      parameter.Description = @"The number of digits to which you want to round. Negative rounds to the left of the decimal point; zero or omitted, to the nearest integer.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROUNDDOWN", Description = @"Rounds a number down, toward zero.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // ROUNDUP
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number that you want rounded up.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfDigits";
      parameter.Display = @"NumberOfDigits";
      parameter.Description = @"The number of digits to which you want to round. Negative rounds to the left of the decimal point; zero or omitted, to the nearest integer.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROUNDUP", Description = @"Rounds a number up, away from zero.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // SIGN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SIGN", Description = @"Returns the sign of a number: 1 if the number is positive, zero if the number is zero, or -1 if the number is negative.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // SIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The angle in radians for which you want the sine.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SIN", Description = @"Returns the sine of the given angle.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // SINH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SINH", Description = @"Returns the hyperbolic sine of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // SQRT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number for which you want the square root.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SQRT", Description = @"Returns the square root of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // SQRTPI
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number by which pi is multiplied.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SQRTPI", Description = @"Returns the square root of (number * pi).", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // TAN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The angle in radians for which you want the tangent.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TAN", Description = @"Returns the tangent of the given angle.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // TANH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"Any real number.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TANH", Description = @"Returns the hyperbolic tangent of a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // TRUNC
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number you want to truncate.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfDigits";
      parameter.Display = @"NumberOfDigits";
      parameter.Description = @"A number specifying the precision of the truncation; if omitted, 0 (zero).";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TRUNC", Description = @"Truncates a number to an integer by removing the decimal, or fractional, part of the number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // BLANK
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"BLANK", Description = @"Returns a blank.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // CONCATENATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text1";
      parameter.Display = @"Text1";
      parameter.Description = @"The first text string to be joined into a single text string. Strings can include text or numbers.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Text2";
      parameter.Display = @"Text2";
      parameter.Description = @"The first text string to be joined into a single text string. Strings can include text or numbers.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CONCATENATE", Description = @"Joins two text strings into one text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // CONCATENATEX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Delimiter";
      parameter.Display = @"Delimiter";
      parameter.Description = @"The delimiter to be concatenated with expression.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"OrderBy_Expression";
      parameter.Display = @"OrderBy_Expression";
      parameter.Description = @"Expression to be used for sorting the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CONCATENATEX", Description = @"Evaluates expression for each row on the table, then return the concatenation of those values in a single string result, seperated by the specified delimiter.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // EXACT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text1";
      parameter.Display = @"Text1";
      parameter.Description = @"The first text string.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Text2";
      parameter.Display = @"Text2";
      parameter.Description = @"The second text string.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EXACT", Description = @"Checks whether two text strings are exactly the same, and returns TRUE or FALSE. EXACT is case-sensitive.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // FIND
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"FindText";
      parameter.Display = @"FindText";
      parameter.Description = @"The text you want to find. Use double quotes (empty text) to match the first character in within_text; wildcard characters not allowed.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"WithinText";
      parameter.Display = @"WithinText";
      parameter.Description = @"The text containing the text you want to find.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"StartPosition";
      parameter.Display = @"StartPosition";
      parameter.Description = @"The character at which to start the search; if omitted, StartPosition = 1. The first character in WithinText is character number 1.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NotFoundValue";
      parameter.Display = @"NotFoundValue";
      parameter.Description = @"The numeric value to be returned if the text is not found; if omitted, an error is returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FIND", Description = @"Returns the starting position of one text string within another text string. FIND is case-sensitive.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // FIXED
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number you want to round and convert to text.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Decimals";
      parameter.Display = @"Decimals";
      parameter.Description = @"The number of digits to the right of the decimal point; if omitted, 2.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NoCommas";
      parameter.Display = @"NoCommas";
      parameter.Description = @"A logical value: if TRUE, do not display commas in the returned text; if FALSE or omitted, display commas in the returned text.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FIXED", Description = @"Rounds a number to the specified number of decimals and returns the result as text with optional commas.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // FORMAT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"A number, or a formula that evaluates to a numeric value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Format";
      parameter.Display = @"Format";
      parameter.Description = @"A number format that you specify.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FORMAT", Description = @"Converts a value to text in the specified number format.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // LEFT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text string containing the characters you want to extract.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfCharacters";
      parameter.Display = @"NumberOfCharacters";
      parameter.Description = @"The number of characters you want LEFT to extract; if omitted, 1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LEFT", Description = @"Returns the specified number of characters from the start of a text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // LEN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text whose length you want to find. Spaces count as characters.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LEN", Description = @"Returns the number of characters in a text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // LOWER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text you want to convert to lowercase. Characters that are not letters are not changed.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LOWER", Description = @"Converts all letters in a text string to lowercase.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // MID
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text string from which you want to extract the characters.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"StartPosition";
      parameter.Display = @"StartPosition";
      parameter.Description = @"The position of the first character you want to extract. Positions start at 1.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfCharacters";
      parameter.Display = @"NumberOfCharacters";
      parameter.Description = @"The number of characters to return.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MID", Description = @"Returns a string of characters from the middle of a text string, given a starting position and length.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // REPLACE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"OldText";
      parameter.Display = @"OldText";
      parameter.Description = @"The string of text that contains the characters you want to replace.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"StartPosition";
      parameter.Display = @"StartPosition";
      parameter.Description = @"The position of the character in old_text that you want to replace with new_text.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfCharacters";
      parameter.Display = @"NumberOfCharacters";
      parameter.Description = @"The number of characters that you want to replace.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NewText";
      parameter.Display = @"NewText";
      parameter.Description = @"The replacement text.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"REPLACE", Description = @"Replaces part of a text string with a different text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // REPT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text you want to repeat.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfTimes";
      parameter.Display = @"NumberOfTimes";
      parameter.Description = @"A positive number specifying the number of times to repeat text.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"REPT", Description = @"Repeats text a given number of times. Use REPT to fill a cell with a number of instances of a text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // RIGHT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text string that contains the characters you want to extract.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfCharacters";
      parameter.Display = @"NumberOfCharacters";
      parameter.Description = @"The number of characters you want to extract; if omitted, 1.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RIGHT", Description = @"Returns the specified number of characters from the end of a text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // SEARCH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"FindText";
      parameter.Display = @"FindText";
      parameter.Description = @"The text you want to find. You can use the ? and * wildcard characters; use ~? and ~* to find the ? and * characters.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"WithinText";
      parameter.Display = @"WithinText";
      parameter.Description = @"The text in which you want to search for FindText.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"StartPosition";
      parameter.Display = @"StartPosition";
      parameter.Description = @"The character position in WithinText at which you want to start searching. If omitted, the default value is 1.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NotFoundValue";
      parameter.Display = @"NotFoundValue";
      parameter.Description = @"The numeric value to be returned if the text is not found; if omitted, an error is returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SEARCH", Description = @"Returns the starting position of one text string within another text string. SEARCH is not case-sensitive.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // SUBSTITUTE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"A string of text, or a reference to a cell containing text, in which you want to substitute characters.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"OldText";
      parameter.Display = @"OldText";
      parameter.Description = @"The existing text you want to replace. If the case of old_text does not match the case in the existing text, SUBSTITUTE will not replace the text.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NewText";
      parameter.Display = @"NewText";
      parameter.Description = @"The text you want to replace old_text with.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"InstanceNumber";
      parameter.Display = @"InstanceNumber";
      parameter.Description = @"The occurrence of old_text you want to replace. If omitted, every instance of old_text is replaced.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SUBSTITUTE", Description = @"Replaces existing text with new text in a text string.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // TRIM
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text from which you want spaces removed.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TRIM", Description = @"Removes all spaces from a text string except for single spaces between words.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // UNICODE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"Text is the character for which you want the Unicode value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"UNICODE", Description = @"Returns the number (code point) corresponding to the first character of the text.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // UPPER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text you want converted to uppercase, or a reference to a cell that contains a text string.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"UPPER", Description = @"Converts a text string to all uppercase letters.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // VALUE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Text";
      parameter.Display = @"Text";
      parameter.Description = @"The text to be converted.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"VALUE", Description = @"Converts a text string that represents a number to a number.", LibraryName = LibraryName.Scalar, InterfaceName = InterfaceName.Text, Parameters = parameters });

      // CLOSINGBALANCEMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CLOSINGBALANCEMONTH", Description = @"Evaluates the specified expression for the date corresponding to the end of the current month after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // CLOSINGBALANCEQUARTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CLOSINGBALANCEQUARTER", Description = @"Evaluates the specified expression for the date corresponding to the end of the current quarter after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // CLOSINGBALANCEYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CLOSINGBALANCEYEAR", Description = @"Evaluates the specified expression for the date corresponding to the end of the current year after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATEADD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfIntervals";
      parameter.Display = @"NumberOfIntervals";
      parameter.Description = @"The number of intervals to shift.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Interval";
      parameter.Display = @"Interval";
      parameter.Description = @"One of: Day, Month, Quarter, Year.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATEADD", Description = @"Moves the given set of dates by a specified interval.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATESBETWEEN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"StartDate";
      parameter.Display = @"StartDate";
      parameter.Description = @"Start date.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"EndDate";
      parameter.Display = @"EndDate";
      parameter.Description = @"End date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATESBETWEEN", Description = @"Returns the dates between two given dates.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATESINPERIOD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"StartDate";
      parameter.Display = @"StartDate";
      parameter.Description = @"Start date.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfIntervals";
      parameter.Display = @"NumberOfIntervals";
      parameter.Description = @"The number of intervals.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Interval";
      parameter.Display = @"Interval";
      parameter.Description = @"One of: Day, Month, Quarter, Year.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATESINPERIOD", Description = @"Returns the dates from the given period.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATESMTD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATESMTD", Description = @"Returns a set of dates in the month up to current date.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATESQTD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATESQTD", Description = @"Returns a set of dates in the quarter up to current date.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // DATESYTD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATESYTD", Description = @"Returns a set of dates in the year up to current date.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // ENDOFMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ENDOFMONTH", Description = @"Returns the end of month.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // ENDOFQUARTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ENDOFQUARTER", Description = @"Returns the end of quarter.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // ENDOFYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ENDOFYEAR", Description = @"Returns the end of year.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // FIRSTDATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FIRSTDATE", Description = @"Returns first non blank date.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // FIRSTNONBLANK
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The source values.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FIRSTNONBLANK", Description = @"Returns the first value in the column for which the expression has a non blank value.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // LASTDATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LASTDATE", Description = @"Returns last non blank date.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // LASTNONBLANK
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The source values.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LASTNONBLANK", Description = @"Returns the last value in the column for which the expression has a non blank value.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // NEXTDAY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NEXTDAY", Description = @"Returns a next day.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // NEXTMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NEXTMONTH", Description = @"Returns a next month.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // NEXTQUARTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NEXTQUARTER", Description = @"Returns a next quarter.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // NEXTYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NEXTYEAR", Description = @"Returns a next year.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // OPENINGBALANCEMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"OPENINGBALANCEMONTH", Description = @"Evaluates the specified expression for the date corresponding to the end of the previous month after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // OPENINGBALANCEQUARTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"OPENINGBALANCEQUARTER", Description = @"Evaluates the specified expression for the date corresponding to the end of the previous quarter after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // OPENINGBALANCEYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"OPENINGBALANCEYEAR", Description = @"Evaluates the specified expression for the date corresponding to the end of the previous year after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // PARALLELPERIOD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"NumberOfIntervals";
      parameter.Display = @"NumberOfIntervals";
      parameter.Description = @"The number of the intervals.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Interval";
      parameter.Display = @"Interval";
      parameter.Description = @"One of: Day, Month, Quarter, Year.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PARALLELPERIOD", Description = @"Returns a parallel period of dates by the given set of dates and a specified interval.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // PREVIOUSDAY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PREVIOUSDAY", Description = @"Returns a previous day.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // PREVIOUSMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PREVIOUSMONTH", Description = @"Returns a previous month.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // PREVIOUSQUARTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PREVIOUSQUARTER", Description = @"Returns a previous quarter.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // PREVIOUSYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PREVIOUSYEAR", Description = @"Returns a previous year.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // SAMEPERIODLASTYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SAMEPERIODLASTYEAR", Description = @"Returns a set of dates in the current selection from the previous year.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // STARTOFMONTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STARTOFMONTH", Description = @"Returns the start of month.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // STARTOFQUARTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STARTOFQUARTER", Description = @"Returns the start of quarter.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // STARTOFYEAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STARTOFYEAR", Description = @"Returns the start of year.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // TOTALMTD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TOTALMTD", Description = @"Evaluates the specified expression over the interval which begins on the first of the month and ends with the last date in the specified date column after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // TOTALQTD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TOTALQTD", Description = @"Evaluates the specified expression over the interval which begins on the first day of the quarter and ends with the last date in the specified date column after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // TOTALYTD
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"The name of a column containing dates or a one column table containing dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"YearEndDate";
      parameter.Display = @"YearEndDate";
      parameter.Description = @"End of year date.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TOTALYTD", Description = @"Evaluates the specified expression over the interval which begins on the first day of the year and ends with the last date in the specified date column after applying specified filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.DateTime, Parameters = parameters });

      // ADDCOLUMNS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table to which new columns are added.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"The name of the new column to be added.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression for the new column to be added.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ADDCOLUMNS", Description = @"Returns a table with new columns specified by the DAX expressions.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ADDMISSINGITEMS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ShowAll_ColumnName";
      parameter.Display = @"ShowAll_ColumnName";
      parameter.Description = @"ShowAll columns.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"A SummarizeColumns table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to group by or a call to ROLLUP function and ISSUBTOTALCOLUMNS function to specify a list of columns to group by with subtotals.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"FilterTable";
      parameter.Display = @"FilterTable";
      parameter.Description = @"An expression that defines the table from which rows are to be returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ADDMISSINGITEMS", Description = @"Add the rows with empty measure values back.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ALL
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TableNameOrColumnName";
      parameter.Display = @"TableNameOrColumnName";
      parameter.Description = @"The name of an existing table or column.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column in the same base table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ALL", Description = @"Returns all the rows in a table, or all the values in a column, ignoring any filters that might have been applied.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ALLEXCEPT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TableName";
      parameter.Display = @"TableName";
      parameter.Description = @"The name of an existing table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column whose filtering is to be retained.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ALLEXCEPT", Description = @"Returns all the rows in a table except for those rows that are affected by the specified column filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ALLNOBLANKROW
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TableNameOrColumnName";
      parameter.Display = @"TableNameOrColumnName";
      parameter.Description = @"A table or column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ALLNOBLANKROW", Description = @"Returns all the rows except blank row in a table, or all the values in a column, ignoring any filters that might have been applied.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ALLSELECTED
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TableNameOrColumnName";
      parameter.Display = @"TableNameOrColumnName";
      parameter.Description = @"Remove all filters on the specified table or column applied within the query.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ALLSELECTED", Description = @"Returns all the rows in a table, or all the values in a column, ignoring any filters that might have been applied inside the query, but keeping filters that come from outside.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CALCULATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CALCULATE", Description = @"Evaluates an expression in a context modified by filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CALCULATETABLE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table expression to be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Filter";
      parameter.Display = @"Filter";
      parameter.Description = @"A boolean (True/False) expression or a table expression that defines a filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CALCULATETABLE", Description = @"Evaluates a table expression in a context modified by filters.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CALENDAR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"StartDate";
      parameter.Display = @"StartDate";
      parameter.Description = @"The start date in datetime format.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"EndDate";
      parameter.Display = @"EndDate";
      parameter.Description = @"The end date in datetime format.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CALENDAR", Description = @"Returns a table with one column of all dates between StartDate and EndDate.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CALENDARAUTO
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"FiscalYearEndMonth";
      parameter.Display = @"FiscalYearEndMonth";
      parameter.Description = @"An integer from 1 to 12 representing the end month of fiscal year.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CALENDARAUTO", Description = @"Returns a table with one column of dates calculated from the model automatically.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CROSSFILTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"LeftColumnName";
      parameter.Display = @"LeftColumnName";
      parameter.Description = @"Left Column.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"RightColumnName";
      parameter.Display = @"RightColumnName";
      parameter.Description = @"Right Column.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"CrossFilterType";
      parameter.Display = @"CrossFilterType";
      parameter.Description = @"The third argument to the CROSSFILTER function should be 0 for None or 1 for OneWay, or 2 for Both. It is also possible to use words None, OneWay, Both.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CROSSFILTER", Description = @"Specifies cross filtering direction to be used in the evaluation of a DAX expression. The relationship is defined by naming, as arguments, the two columns that serve as endpoints.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CROSSJOIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"A table that will participate in the crossjoin.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"CROSSJOIN", Description = @"Returns a table that is a crossjoin of the specified tables.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // CURRENTGROUP
      parameters = new List<Babel.Parameter>();
      this.Add(new DaxFunctionDescription() { Name = @"CURRENTGROUP", Description = @"Access to the (sub)table representing current group in GroupBy function. Can be used only inside GroupBy function.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // DATATABLE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"name";
      parameter.Display = @"name";
      parameter.Description = @"A column name to be defined.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"type";
      parameter.Display = @"type";
      parameter.Description = @"A type name to be associated with the column.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"data";
      parameter.Display = @"data";
      parameter.Description = @"The data for the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DATATABLE", Description = @"Returns a table with data defined inline.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // DISTINCT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnNameOrTableExpr";
      parameter.Display = @"ColumnNameOrTableExpr";
      parameter.Description = @"The column (or table expression) from which unique values (or combination of values) are to be returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DISTINCT", Description = @"Returns a one column table that contains the distinct (unique) values in a column, for a column argument. Or multiple columns with distinct (unique) combination of values, for a table expression argument.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // EARLIER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the desired value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Number";
      parameter.Display = @"Number";
      parameter.Description = @"The number of table scan.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EARLIER", Description = @"Returns the value in the column prior to the specified number of table scans (default is 1).", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // EARLIEST
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the desired value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EARLIEST", Description = @"Returns the value in the column for the very first point at which there was a row context.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // EXCEPT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"LeftTable";
      parameter.Display = @"LeftTable";
      parameter.Description = @"The Left-side table expression to be used for Except.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"RightTable";
      parameter.Display = @"RightTable";
      parameter.Description = @"The Right-side table expression to be used for Except.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"EXCEPT", Description = @"Returns the rows of left-side table which do not appear in right-side table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // FILTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table to be filtered.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"FilterExpression";
      parameter.Display = @"FilterExpression";
      parameter.Description = @"A boolean (True/False) expression that is to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FILTER", Description = @"Returns a table that has been filtered.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // FILTERS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column for which filter values are to be returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"FILTERS", Description = @"Returns a table of the filter values applied directly to the specified column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // GENERATE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table1";
      parameter.Display = @"Table1";
      parameter.Description = @"The base table in Generate.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Table2";
      parameter.Display = @"Table2";
      parameter.Description = @"A table expression that will be evaluated for each row in the first table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"GENERATE", Description = @"The second table expression will be evaluated for each row in the first table. Returns the crossjoin of the first table with these results.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // GENERATEALL
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table1";
      parameter.Display = @"Table1";
      parameter.Description = @"The base table in Generate.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Table2";
      parameter.Display = @"Table2";
      parameter.Description = @"A table expression that will be evaluated for each row in the first table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"GENERATEALL", Description = @"The second table expression will be evaluated for each row in the first table. Returns the crossjoin of the first table with these results, including rows for which the second table expression is empty.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // GROUPBY
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The input table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to group by.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"A column name to be added.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression of the new column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"GROUPBY", Description = @"Creates a summary the input table grouped by the specified columns.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // IGNORE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Measure_Expression";
      parameter.Display = @"Measure_Expression";
      parameter.Description = @"The Expression which needs to be ignored for the purposes of determining non-blank rows.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"IGNORE", Description = @"Tags a measure expression specified in the call to SUMMARIZECOLUMNS function to be ignored when determining the non-blank rows.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // INTERSECT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"LeftTable";
      parameter.Display = @"LeftTable";
      parameter.Description = @"The Left-side table expression to be used for Intersect.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"RightTable";
      parameter.Display = @"RightTable";
      parameter.Description = @"The Right-side table expression to be used for Intersect.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"INTERSECT", Description = @"Returns the rows of left-side table which appear in right-side table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ISONORAFTER
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value1";
      parameter.Display = @"Value1";
      parameter.Description = @"Expression to be compared with second parameter.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Value2";
      parameter.Display = @"Value2";
      parameter.Description = @"Expression to be compared with first parameter.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ISONORAFTER", Description = @"The IsOnOrAfter function is a boolean function that emulates the behavior of Start At clause and returns true for a row that meets all the conditions mentioned as parameters in this function.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // KEEPFILTERS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"CALCULATE or CALCULATETABLE function expression or filter.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"KEEPFILTERS", Description = @"Changes the CALCULATE and CALCULATETABLE function filtering semantics.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // LOOKUPVALUE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Result_ColumnName";
      parameter.Display = @"Result_ColumnName";
      parameter.Description = @"The column that contains the desired value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Search_ColumnName";
      parameter.Display = @"Search_ColumnName";
      parameter.Description = @"The column that contains search_value.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Search_Value";
      parameter.Display = @"Search_Value";
      parameter.Description = @"The value that you want to find in search_column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"LOOKUPVALUE", Description = @"Retrieves a value from a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // NATURALINNERJOIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"LeftTable";
      parameter.Display = @"LeftTable";
      parameter.Description = @"The Left-side table expression to be used for join.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"RightTable";
      parameter.Display = @"RightTable";
      parameter.Description = @"The Right-side table expression to be used for join.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NATURALINNERJOIN", Description = @"Joins the Left table with right table using the Inner Join semantics.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // NATURALLEFTOUTERJOIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"LeftTable";
      parameter.Display = @"LeftTable";
      parameter.Description = @"The Left-side table expression to be used for join.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"RightTable";
      parameter.Display = @"RightTable";
      parameter.Description = @"The Right-side table expression to be used for join.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"NATURALLEFTOUTERJOIN", Description = @"Joins the Left table with right table using the Left Outer Join semantics.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // RELATED
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the desired value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RELATED", Description = @"Returns a related value from another table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // RELATEDTABLE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table that contains the desired value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RELATEDTABLE", Description = @"Returns the related tables filtered so that it only includes the related rows.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ROLLUP
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to be returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROLLUP", Description = @"Identifies a subset of columns specified in the call to SUMMARIZE function that should be used to calculate subtotals.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ROLLUPADDISSUBTOTAL
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to be returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"A column name to be added.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROLLUPADDISSUBTOTAL", Description = @"Identifies a subset of columns specified in the call to SUMMARIZECOLUMNS function that should be used to calculate groups of subtotals.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ROLLUPGROUP
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to be returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROLLUPGROUP", Description = @"Identifies a subset of columns specified in the call to SUMMARIZE function that should be used to calculate groups of subtotals.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ROLLUPISSUBTOTAL
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to be returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"IsSubtotal_ColumnName";
      parameter.Display = @"IsSubtotal_ColumnName";
      parameter.Description = @"An added IsSubtotal column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROLLUPISSUBTOTAL", Description = @"Pairs up the rollup groups with the column added by ROLLUPADDISSUBTOTAL.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // ROW
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"Name of the new column.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression for the column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"ROW", Description = @"Returns a single row table with new columns specified by the DAX expressions.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // SAMPLE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Size";
      parameter.Display = @"Size";
      parameter.Description = @"Number of rows in the sample to be returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"A table expression from which the sample is generated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"OrderBy";
      parameter.Display = @"OrderBy";
      parameter.Description = @"A scalar expression evaluated for each row of the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SAMPLE", Description = @"Returns a sample subset from a given table expression.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // SELECTCOLUMNS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table from which columns are selected.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"The name of the new column to be added.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression for the new column to be added.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SELECTCOLUMNS", Description = @"Returns a table with selected columns from the table and new columns specified by the DAX expressions.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // SUBSTITUTEWITHINDEX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table to be modified.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"A name of the column to be added to the first table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"SemiJoinIndexTable";
      parameter.Display = @"SemiJoinIndexTable";
      parameter.Description = @"Table that will be ordered and used to calculate index and to join with the first argument.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"Order by expression for the second parameter (SemiJoinIndexTable).";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SUBSTITUTEWITHINDEX", Description = @"Returns a table which represents the semijoin of two tables supplied and for which the common set of columns are replaced by a 0-based index column. The index is based on the rows of the second table sorted by specified order expressions.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // SUMMARIZE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The input table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to group by or a call to ROLLUP function to specify a list of columns to group by with subtotals.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"A column name to be added.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression of the new column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SUMMARIZE", Description = @"Creates a summary the input table grouped by the specified columns.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // SUMMARIZECOLUMNS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"GroupBy_ColumnName";
      parameter.Display = @"GroupBy_ColumnName";
      parameter.Description = @"A column to group by or a call to ROLLUP function and ISSUBTOTALCOLUMNS function to specify a list of columns to group by with subtotals.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"FilterTable";
      parameter.Display = @"FilterTable";
      parameter.Description = @"An expression that defines the table from which rows are to be returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Name";
      parameter.Display = @"Name";
      parameter.Description = @"A column name to be added.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression of the new column.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SUMMARIZECOLUMNS", Description = @"Create a summary table for the requested totals over set of groups.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // TOPN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"N_Value";
      parameter.Display = @"N_Value";
      parameter.Description = @"The number of rows to return.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"An expression that defines the table from which rows are to be returned.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"OrderBy_Expression";
      parameter.Display = @"OrderBy_Expression";
      parameter.Description = @"Expression to be used for sorting the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"TOPN", Description = @"Returns a given number of top rows according to a specified expression.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // UNION
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"A table that will participate in the crossjoin union.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"UNION", Description = @"Returns the union of the two tables whose columns match.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // USERELATIONSHIP
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName1";
      parameter.Display = @"ColumnName1";
      parameter.Description = @"Foreign (or primary) key of the relationship.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName2";
      parameter.Display = @"ColumnName2";
      parameter.Description = @"Primary (or foreign) key of the relationship.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"USERELATIONSHIP", Description = @"Specifies an existing relationship to be used in the evaluation of a DAX expression. The relationship is defined by naming, as arguments, the two columns that serve as endpoints.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // VALUES
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"TableNameOrColumnName";
      parameter.Display = @"TableNameOrColumnName";
      parameter.Description = @"The column or table from which unique values are to be returned.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"VALUES", Description = @"Returns a one column table or a table that contains the distinct (unique) values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Filter, Parameters = parameters });

      // SUM
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the numbers to sum.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SUM", Description = @"Adds all the numbers in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // SUMX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"SUMX", Description = @"Returns the sum of an expression evaluated for each row in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.MathTrig, Parameters = parameters });

      // PATH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ID_ColumnName";
      parameter.Display = @"ID_ColumnName";
      parameter.Description = @"The column containing the IDs for each row.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Parent_ColumnName";
      parameter.Display = @"Parent_ColumnName";
      parameter.Description = @"The column containing the parent IDs for each row.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PATH", Description = @"Returns a string which contains a delimited list of IDs, starting with the top/root of a hierarchy and ending with the specified ID.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.ParentChild, Parameters = parameters });

      // PATHCONTAINS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Path";
      parameter.Display = @"Path";
      parameter.Description = @"A string which contains a delimited list of IDs.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Item";
      parameter.Display = @"Item";
      parameter.Description = @"A value to be found in the path.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PATHCONTAINS", Description = @"Returns TRUE if the specified Item exists within the specified Path.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.ParentChild, Parameters = parameters });

      // PATHITEM
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Path";
      parameter.Display = @"Path";
      parameter.Description = @"A string which contains a delimited list of IDs.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Position";
      parameter.Display = @"Position";
      parameter.Description = @"An integer denoting the position from the left end of the path.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Type";
      parameter.Display = @"Type";
      parameter.Description = @"Optional. If missing or 0 then this function returns a string. If 1 then this function returns an integer.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PATHITEM", Description = @"Returns the nth item in the delimited list produced by the Path function.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.ParentChild, Parameters = parameters });

      // PATHITEMREVERSE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Path";
      parameter.Display = @"Path";
      parameter.Description = @"A string which contains a delimited list of IDs.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Position";
      parameter.Display = @"Position";
      parameter.Description = @"An integer denoting the position from the right end of the path.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Type";
      parameter.Display = @"Type";
      parameter.Description = @"Optional. If missing or 0 then this function returns a string. If 1 then this function returns an integer.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PATHITEMREVERSE", Description = @"Returns the nth item in the delimited list produced by the Path function, counting backwards from the last item in the path.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.ParentChild, Parameters = parameters });

      // PATHLENGTH
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Path";
      parameter.Display = @"Path";
      parameter.Description = @"A string which contains a delimited list of IDs.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PATHLENGTH", Description = @"Returns returns the number of items in a particular path string.  This function returns 1 for the path generated for an ID at the top/root of a hierarchy.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.ParentChild, Parameters = parameters });

      // AVERAGE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the numbers for which you want the average.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"AVERAGE", Description = @"Returns the average (arithmetic mean) of all the numbers in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // AVERAGEA
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column that contains the values for which you want the average.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"AVERAGEA", Description = @"Returns the average (arithmetic mean) of all the values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // AVERAGEX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"AVERAGEX", Description = @"Returns the average (arithmetic mean) of all the numbers in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // COUNT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the numbers to be counted.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COUNT", Description = @"Counts the numbers in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // COUNTA
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column that contains the values to be counted.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COUNTA", Description = @"Counts the number of values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // COUNTAX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COUNTAX", Description = @"Counts the number of values which result from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // COUNTBLANK
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column containing the blanks to be counted.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COUNTBLANK", Description = @"Counts the number of blanks in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // COUNTROWS
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows to be counted.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COUNTROWS", Description = @"Counts the number of rows in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // COUNTX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"COUNTX", Description = @"Counts the number of values which result from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // DISTINCTCOUNT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column for which the distinct values are counted.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DISTINCTCOUNT", Description = @"Counts the number of distinct values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // DIVIDE
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Numerator";
      parameter.Display = @"Numerator";
      parameter.Description = @"Numerator.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Denominator";
      parameter.Display = @"Denominator";
      parameter.Description = @"Denominator.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"AlternateResult";
      parameter.Display = @"AlternateResult";
      parameter.Description = @"Optional. The alternate result to return when dividing by zero.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"DIVIDE", Description = @"Safe Divide function with ability to handle divide by zero case.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // GEOMEAN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"Column that contains values for geometric mean.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"GEOMEAN", Description = @"Returns geometric mean of given column reference.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // GEOMEANX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table over which the Expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"Expression to evaluate for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"GEOMEANX", Description = @"Returns geometric mean of an expression values in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MAX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnNameOrScalar1";
      parameter.Display = @"ColumnNameOrScalar1";
      parameter.Description = @"The column in which you want to find the largest numeric value, or the first scalar expression to compare.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Scalar2";
      parameter.Display = @"Scalar2";
      parameter.Description = @"The second number to compare.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MAX", Description = @"Returns the largest numeric value in a column, or the larger value between two scalar expressions. Ignores logical values and text.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MAXA
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column in which you want to find the largest value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MAXA", Description = @"Returns the largest value in a column. Does not ignore logical values and text.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MAXX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MAXX", Description = @"Returns the largest numeric value that results from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MEDIAN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Column";
      parameter.Display = @"Column";
      parameter.Description = @"A column containing the values.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MEDIAN", Description = @"Returns the 50th percentile of values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MEDIANX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table over which the Expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"Expression to evaluate for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MEDIANX", Description = @"Returns the 50th percentile of an expression values in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MIN
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnNameOrScalar1";
      parameter.Display = @"ColumnNameOrScalar1";
      parameter.Description = @"The column in which you want to find the smallest numeric value, or the first scalar expression to compare.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Scalar2";
      parameter.Display = @"Scalar2";
      parameter.Description = @"The second number to compare.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MIN", Description = @"Returns the smallest numeric value in a column, or the smaller value between two scalar expressions. Ignores logical values and text.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MINA
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"The column for which you want to find the smallest value.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MINA", Description = @"Returns the smallest value in a column. Does not ignore logical values and text.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // MINX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"MINX", Description = @"Returns the smallest numeric value that results from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // PERCENTILE.EXC
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Column";
      parameter.Display = @"Column";
      parameter.Description = @"A column containing the values.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"K";
      parameter.Display = @"K";
      parameter.Description = @"Desired percentile value in the interval [1/(n+1),1-1/(n+1)], where n is a number of valid data points.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PERCENTILE.EXC", Description = @"Returns the k-th (exclusive) percentile of values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // PERCENTILE.INC
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Column";
      parameter.Display = @"Column";
      parameter.Description = @"A column containing the values.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"K";
      parameter.Display = @"K";
      parameter.Description = @"Desired percentile value in the interval [0,1].";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PERCENTILE.INC", Description = @"Returns the k-th (inclusive) percentile of values in a column.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // PERCENTILEX.EXC
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table over which the Expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"Expression to evaluate for each row of the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"K";
      parameter.Display = @"K";
      parameter.Description = @"Desired percentile value in the interval [1/(n+1),1-1/(n+1)], where n is a number of valid data points.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PERCENTILEX.EXC", Description = @"Returns the k-th (exclusive) percentile of an expression values in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // PERCENTILEX.INC
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table over which the Expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"Expression to evaluate for each row of the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"K";
      parameter.Display = @"K";
      parameter.Description = @"Desired percentile value in the interval [0,1].";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PERCENTILEX.INC", Description = @"Returns the k-th (inclusive) percentile of an expression values in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // PRODUCT
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"Column that contains values for product.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PRODUCT", Description = @"Returns the product of given column reference.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // PRODUCTX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"Table over which the Expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"Expression to evaluate for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"PRODUCTX", Description = @"Returns the product of an expression values in a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // RANK.EQ
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"The value to be ranked.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column in which the value will be ranked.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RANK.EQ", Description = @"Returns the rank of a number in a column of numbers. If more than one value has the same rank, the top rank of that set of values is returned.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // RANKX
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"A table expression.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"An expression that will be evaluated for row of the table.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Value";
      parameter.Display = @"Value";
      parameter.Description = @"An expression that will be evaluated in the current context.  If omitted, the Expression argument will be used.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Order";
      parameter.Display = @"Order";
      parameter.Description = @"The order to be applied. 0/FALSE/DESC - descending; 1/TRUE/ASC - ascending.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Ties";
      parameter.Display = @"Ties";
      parameter.Description = @"Function behavior in the event of ties. Skip - ranks that correspond to elements in ties will be skipped; Dense - all elements in a tie are counted as one.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"RANKX", Description = @"Returns the rank of an expression evaluated in the current context in the list of values for the expression evaluated for each row in the specified table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // STDEV.P
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column that contains numbers corresponding to a population.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STDEV.P", Description = @"Calculates standard deviation based on the entire population given as arguments. Ignores logical values and text.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // STDEV.S
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column that contains numbers corresponding to a sample of a population.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STDEV.S", Description = @"Estimates standard deviation based on a sample. Ignores logical values and text in the sample.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // STDEVX.P
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STDEVX.P", Description = @"Estimates standard deviation based on the entire population that results from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // STDEVX.S
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"STDEVX.S", Description = @"Estimates standard deviation based on a sample that results from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // VAR.P
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column that contains values corresponding to a population.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"VAR.P", Description = @"Calculates variance based on the entire population. Ignores logical values and text in the population.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // VAR.S
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"ColumnName";
      parameter.Display = @"ColumnName";
      parameter.Description = @"A column that contains numeric values corresponding to a sample of a population.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"VAR.S", Description = @"Estimates variance based on a sample. Ignores logical values and text in the sample.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // VARX.P
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"VARX.P", Description = @"Estimates variance based on the entire population that results from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // VARX.S
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the expression will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Expression";
      parameter.Display = @"Expression";
      parameter.Description = @"The expression to be evaluated for each row of the table.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"VARX.S", Description = @"Estimates variance based on a sample that results from evaluating an expression for each row of a table.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // XIRR
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the Values and Dates expressions will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Values";
      parameter.Display = @"Values";
      parameter.Description = @"An expression to be evaluated for each row of the table, which will yield a series of cash flows.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"An expression to be evaluated for each row of the table, which will yield a schedule of payment dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Guess";
      parameter.Display = @"Guess";
      parameter.Description = @"Optional.  A number that you guess is close to the result of XIRR.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"XIRR", Description = @"Returns the internal rate of return for a schedule of cash flows that is not necessarily periodic.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });

      // XNPV
      parameters = new List<Babel.Parameter>();
      parameter = new Babel.Parameter();
      parameter.Name = @"Table";
      parameter.Display = @"Table";
      parameter.Description = @"The table containing the rows for which the Values and Dates expressions will be evaluated.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Values";
      parameter.Display = @"Values";
      parameter.Description = @"An expression to be evaluated for each row of the table, which will yield a series of cash flows.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Dates";
      parameter.Display = @"Dates";
      parameter.Description = @"An expression to be evaluated for each row of the table, which will yield a schedule of payment dates.";
      parameters.Add(parameter);
      parameter = new Babel.Parameter();
      parameter.Name = @"Rate";
      parameter.Display = @"Rate";
      parameter.Description = @"The discount rate to apply to the cash flows.";
      parameters.Add(parameter);
      this.Add(new DaxFunctionDescription() { Name = @"XNPV", Description = @"Returns the net present value for a schedule of cash flows.", LibraryName = LibraryName.Unknown, InterfaceName = InterfaceName.Statistical, Parameters = parameters });
// End of generated code
//
        }
    }
}

















