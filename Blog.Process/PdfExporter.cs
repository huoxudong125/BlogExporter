using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Blog.Common.Entities;
using Blog.Process.Interfaces;

namespace Blog.Process
{
    public class PdfExporter : IExporter
    {
        //baseDir

        public Task Export(string fileName, List<Article> articles, IBlogProcess processer,
            IProgress<DownloadStringTaskAsyncExProgress> progress = null)
        {
            throw new NotImplementedException();
        }

        private void init()
        {
            //if (File.Exists(baseDir + _fileName))
            //{
            //    File.Delete(baseDir + _fileName);
            //}
        }
    }
}