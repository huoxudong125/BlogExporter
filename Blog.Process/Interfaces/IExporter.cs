using System.Collections.Generic;
using Blog.Common;

namespace Blog.Process.Interfaces
{
    public interface IExporter
    {
        bool Export(List<Article> urls);
    }
}