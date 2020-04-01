using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectBoundBindingList.LinqExtension
{
    internal static class InnerFunctions
    {
        public static string substring(string source, int start, int ende)
        {
            return source.Substring(start, ende);
        }

        public static string IsNull(object t, string value)
        {
            if (t == null)
                return value;
            return t.ToString();
        }

        public static string Trim(string t)
        {
            return t.Trim();
        }
    }
}
