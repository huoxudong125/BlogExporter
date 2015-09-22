using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Common.Entities;
using Blog.Process.Interfaces;
using HtmlAgilityPack;
using ScrapySharp.Network;

namespace Blog.Process
{
    public abstract class BlogProcessBase : IBlogProcess
    {
        private ScrapingBrowser _scrapyBrowser;

        public BlogProcessBase()
        {
            _scrapyBrowser = new ScrapingBrowser();
        }

        public abstract  Task<List<Catalog>> ParseCatalogs(string catalogUrl);

        public abstract Task<bool> ExtractArticleContent(Article article
            , IProgress<DownloadStringTaskAsyncExProgress> progress);

        #region Protected

        protected HtmlNode GetHtmlNodeByUrl(string catalogUrl)
        {
            var html1 = _scrapyBrowser.DownloadString(new Uri(catalogUrl));
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html1);
            var html = htmlDocument.DocumentNode;
            return html;
        }

        #endregion Protected
    }
}