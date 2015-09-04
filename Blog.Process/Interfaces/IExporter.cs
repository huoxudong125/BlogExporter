using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Common;

namespace Blog.Process.Interfaces
{
    public interface IExporter
    {
        Task Export(List<Article> urls);

        Task Export(List<Article> urls, IProgress<DownloadStringTaskAsyncExProgress> progress);
    }
}