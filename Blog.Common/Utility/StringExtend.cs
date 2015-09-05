using System;

namespace Blog.Common
{
    public static class StringExtend
    {
        /// <summary>
        /// Clear tab char & Clear NewLine
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ClearNotWords(this string str)
        {
            return str.Replace(@"\t", String.Empty).Replace(@"\n", String.Empty).Trim();
        }

        /// <summary>
        /// Make the FileName is validate name, By removing invalid chars.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
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