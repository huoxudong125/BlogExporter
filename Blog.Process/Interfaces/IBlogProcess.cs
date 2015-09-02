using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Process.Interfaces
{
    public interface IBlogProcess
    {
        bool ParseCatalog();

        bool ParseArticle();
    }
}
