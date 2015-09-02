using System;

namespace BlogExporter.Shell.Utility
{
    internal static class StringExtend
    {
        public static string ClearNotWords(this string str)
        {
            return str.Replace(@"\t", String.Empty).Replace(@"\n", String.Empty).Trim();
        }

        public static string ToValidFileName(this string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }
            return fileName;
        }
    }
}