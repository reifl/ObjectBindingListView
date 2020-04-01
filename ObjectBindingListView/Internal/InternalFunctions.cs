using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView.Internal
{
    internal static class InternalFunctions
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
