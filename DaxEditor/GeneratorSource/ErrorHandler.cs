/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Babel.ParserGenerator;

namespace Babel.Parser
{
    public class Error : IComparable<Error>
    {
        public string message;
        public int line;
        public int column;
        public int length;
        public bool isWarn;

        internal Error(string msg, int lin, int col, int len, bool warningOnly)
        {
            isWarn = warningOnly;
            message = msg;
            line = lin;
            column = col;
            length = len;
        }

        public int CompareTo(Error r)
        {
            if (this.line < r.line) return -1;
            else if (this.line > r.line) return 1;
            else if (this.column < r.column) return -1;
            else if (this.column > r.column) return 1;
            else return 0;
        }

        public bool Equals(Error r)
        {
            return (this.line == r.line && this.column == r.column);
        }

        public void Report()
        {
            Console.WriteLine("Line " + line + ", column  " + column + ": " + message);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}): {3}: {2}", line, column, message, isWarn? "warning" : "error");
        }
    }

    public class Span
    {
        public int sLin;
        public int sCol;
        public int eLin;
        public int eCol;
        public int sPos;
        public int leng;
        public Span(int sl, int sc, int el, int ec, int ps, int lg)
        { 
            sLin = sl; sCol = sc; eLin = el; eCol = ec; sPos = ps; leng = lg; 
        }
        public static Span Merge(Span lh, Span rh)
        {
            return new Span(lh.sLin, lh.sCol, rh.eLin, rh.eCol, lh.sPos, rh.sPos - lh.sPos + rh.leng);
        }

    }
    
    
    public class ErrorHandler : IErrorHandler
    {
        const int errLev = 2; 

        List<Error> errors;
        int errNum = 0;
        int wrnNum = 0; 

        public bool Errors { get { return errNum > 0; } }
        public bool Warnings { get { return wrnNum > 0; } }
        public int ErrNum { get { return errNum; } }
        public int WrnNum { get { return wrnNum; } }

        public ErrorHandler()
        {
            errors = new List<Error>(8);
        }
        // -----------------------------------------------------
        //   Public utility methods
        // -----------------------------------------------------

        public List<Error> SortedErrorList()
        {
            if (errors.Count > 0)
            {
                errors.Sort();
                return errors;
            }
            else
            {
                return null;
            }
        }

        public void AddError(string msg, LexLocation span, int severity)
        {
            AddError(msg, span.sLin, span.sCol, span.eCol - span.sCol + 1, severity);
        }

        public void AddError(string msg, int lin, int col, int len, int severity)
        {
            bool warnOnly = severity < errLev;
            errors.Add(new Error(msg, lin, col, len, warnOnly));
            if (warnOnly) wrnNum++; else errNum++;
        }

        public void AddError(string msg, int lin, int col, int len)
        {
            errors.Add(new Error(msg, lin, col, len, false)); errNum++;
        }

        public void AddWarning(string msg, int lin, int col, int len)
        {
            errors.Add(new Error(msg, lin, col, len, true)); wrnNum++;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            errors.ForEach(i => sb.AppendLine(i.ToString()));
            return sb.ToString();
        }

    }
}