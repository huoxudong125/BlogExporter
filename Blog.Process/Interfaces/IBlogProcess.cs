using Blog.Common.Entities;

namespace Blog.Process.Interfaces
{
    public interface IBlogProcess
    {
        Catalog ParseCatalog(string catalogUrl);

        Article ParseArticle(string articleUrl);
    }
}