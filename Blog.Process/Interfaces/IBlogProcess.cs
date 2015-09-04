namespace Blog.Process.Interfaces
{
    public interface IBlogProcess
    {
        bool ParseCatalog();

        bool ParseArticle();
    }
}