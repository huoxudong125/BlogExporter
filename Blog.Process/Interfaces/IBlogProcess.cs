using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Common.Entities;

namespace Blog.Process.Interfaces
{
    public interface IBlogProcess
    {
        Task<List<Catalog>> ParseCatalogs(string catalogUrl);

        Task<bool> ExtractArticleContent(Article article
            , IProgress<DownloadStringTaskAsyncExProgress> progress);
    }
}