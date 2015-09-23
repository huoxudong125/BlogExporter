using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Common.Entities;
using Blog.Process.Interfaces;

namespace Blog.Process
{
    public class PdfExporter : IExporter
    {
        public Task Export(string fileName, List<Article> articles, IBlogProcess processer,
            IProgress<DownloadStringTaskAsyncExProgress> progress = null)
        {
            throw new NotImplementedException();
        }
    }
}