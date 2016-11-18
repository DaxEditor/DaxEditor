// The project released under MS-PL license https://daxeditor.codeplex.com/license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace DaxEditor
{
    public class ConsoleToDebugRedirector : TextWriter
    {
        public override void Write(bool value)
        {
            Debug.Write(value);
        }
        public override void Write(char value)
        {
            Debug.Write(value);
        }
        public override void Write(char[] buffer)
        {
            Debug.Write(buffer);
        }
        public override void Write(decimal value)
        {
            Debug.Write(value);
        }
        public override void Write(double value)
        {
            Debug.Write(value);
        }
        public override void Write(float value)
        {
            Debug.Write(value);
        }
        public override void Write(int value)
        {
            Debug.Write(value);
        }
        public override void Write(long value)
        {
            Debug.Write(value);
        }
        public override void Write(object value)
        {
            Debug.Write(value);
        }
        public override void Write(string value)
        {
            Debug.Write(value);
        }
        public override void Write(string format, object arg0)
        {
            Debug.Write(string.Format(format, arg0));
        }
        public override void Write(string format, params object[] arg)
        {
            Debug.Write(string.Format(format, arg));
        }
        public override void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }
        public override void Write(string format, object arg0, object arg1)
        {
            Debug.Write(string.Format(format, arg0, arg1));
        }
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            Debug.Write(string.Format(format, arg0, arg1, arg2));
        }
        public override void WriteLine()
        {
            Debug.WriteLine("");
        }
        public override void WriteLine(bool value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(char value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(char[] buffer)
        {
            Debug.WriteLine(buffer);
        }
        public override void WriteLine(decimal value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(double value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(float value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(int value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(long value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(object value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }
        public override void WriteLine(string format, object arg0)
        {
            Debug.WriteLine(string.Format(format, arg0));
        }
        public override void WriteLine(string format, params object[] arg)
        {
            Debug.WriteLine(string.Format(format, arg));
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            Debug.WriteLine(string.Format(format, arg0, arg1));
        }
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Debug.WriteLine(string.Format(format, arg0, arg1, arg2));
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}

