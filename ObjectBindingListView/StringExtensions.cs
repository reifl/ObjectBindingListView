using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBindingListView
{
    internal static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }

        public static bool EndsWidth(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) == source.Length - toCheck.Length;
        }

        public static bool StartWisth(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) == 0;
        }
    }
}
