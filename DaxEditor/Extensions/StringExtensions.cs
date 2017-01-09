using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaxEditor.StringExtensions
{
    public static class StringExtensions
    {
        public static string ToUnixEnding(this string value)
        {
            return value.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string ToSystemEnding(this string value)
        {
            return value.ToUnixEnding().Replace("\n", Environment.NewLine);
        }

        public static string ToWindowsEnding(this string value)
        {
            return value.ToUnixEnding().Replace("\n", "\r\n");
        }

        public static string ToEnding(this string value, string ending)
        {
            return value.ToUnixEnding().Replace("\n", ending);
        }

        public static IEnumerable<string> ToLines(this string value)
        {
            return value.ToUnixEnding().Split('\n');
        }
    }
}
