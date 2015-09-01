using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogExporter.Shell.Utility
{
    internal static class StringExtend
    {
        public static string ClearNotWords(this string str)
        {
            return str.Replace(@"\t", String.Empty).Replace(@"\n", String.Empty).Trim();
        }
    }
}
