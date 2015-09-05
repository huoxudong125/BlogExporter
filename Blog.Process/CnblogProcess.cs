using System;
using Blog.Common.Entities;

namespace Blog.Process
{
    public class CnblogProcess : BlogProcessBase
    {
        public CnblogProcess()
        {
            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
        }

        public override Catalog ParseCatalog(string catalogUrl)
        {
            throw new NotImplementedException();
        }

        public override Article ParseArticle(string articleUrl)
        {
            throw new NotImplementedException();
        }
    }
}