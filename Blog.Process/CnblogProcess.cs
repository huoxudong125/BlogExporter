using System;
using Blog.Common;
using Blog.Common.Entities;
using Blog.Process.Interfaces;

namespace Blog.Process
{
    public class CnblogProcess : IBlogProcess
    {
        public Catalog ParseCatalog(string catalogUrl)
        {
            throw new NotImplementedException();
        }

        public Article ParseArticle(string articleUrl)
        {
            throw new NotImplementedException();
        }
    }
}