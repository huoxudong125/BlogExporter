using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Common.Entities;

namespace Blog.Process.Interfaces
{
    public interface IExporter
    {

        Task Export(string fileName,List<Article> articles, IBlogProcess processer
            , IProgress<DownloadStringTaskAsyncExProgress> progress = null);
    }
}