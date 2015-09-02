using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Process.Interfaces;

namespace Blog.Process
{
    public class CnblogProcess:IBlogProcess,IExporter
    {
        public bool ParseCatalog()
        {
            throw new NotImplementedException();
        }

        public bool ParseArticle()
        {
            throw new NotImplementedException();
        }

        public bool Export()
        {
            throw new NotImplementedException();
        }
    }
}
