using System.Collections.Generic;
using Blog.Common.Entities;

namespace Blog.Process.Interfaces
{
    public interface IBlogProcess
    {
        List<Catalog> ParseCatalogs(string catalogUrl);

        List<Article> ParseArticles(string articleUrl);
    }
}