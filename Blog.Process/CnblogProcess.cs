using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Common;
using Blog.Common.Entities;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace Blog.Process
{
    public class CnblogProcess : BlogProcessBase
    {
        private string CatalogsUrlTemplate=@"http://www.cnblogs.com/{0}/default.aspx?page={1}&onlytitle=1";

        public CnblogProcess()
        {
            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
        }

        public async override Task<List<Catalog>> ParseCatalogs(string blogerName)
        {
            await Task.Yield();

            var url = string.Format(CatalogsUrlTemplate, blogerName, 1);
            var webIncludeCount = string.Format(CatalogsUrlTemplate, blogerName,2);
            var catalogPageCount=await ExtractCatalogPageCount(webIncludeCount);

            List < Catalog > resultCatalogs=new List<Catalog>();
            for (int i = 1; i < catalogPageCount; i++)
            {
                url= string.Format(CatalogsUrlTemplate, blogerName, i);
                resultCatalogs.AddRange(await ExtractCatalogsFromWebPage(url).ConfigureAwait(false)); 
            }

            return resultCatalogs;

        }


        private async Task<int> ExtractCatalogPageCount(string url)
        {
            int pageCount = 1;
            var uri = new Uri(url);
            var browser1 = new ScrapingBrowser();
            var html1 = browser1.DownloadString(uri);
            var doc = new HtmlDocument();
            doc.LoadHtml(html1);
            var html = doc.DocumentNode;

            var pagers =await Task.Run(()=>html.CssSelect("div.pager")).ConfigureAwait(false);

            if (pagers.Count() > 1)
            {
         
                var mp = Regex.Match(pagers.First().InnerText, @"共(\d+)页");
                if (mp.Success) pageCount= int.Parse(mp.Groups[1].Value);
            }

            return pageCount;
        }

        private async Task<List<Catalog>> ExtractCatalogsFromWebPage(string url)
        {
            List<Catalog> reusltCatalogs = new List<Catalog>();

            var uri = new Uri(url);
            var browser1 = new ScrapingBrowser();
            var html1 = browser1.DownloadString(uri);
            var doc = new HtmlDocument();
            doc.LoadHtml(html1);
            var html = doc.DocumentNode;

            foreach (var script in doc.DocumentNode.Descendants("script").ToArray())
            {
                script.Remove();
            }
            foreach (var style in doc.DocumentNode.Descendants("style").ToArray())
            {
                style.Remove();
            }
            foreach (var comment in doc.DocumentNode.SelectNodes("//comment()").ToArray())
            {
                comment.Remove();
            }

            var days = html.CssSelect("div.day");
            foreach (var day in days)
            {
                var title = day.CssSelect("div.dayTitle").First();
                var catalog = new Catalog();

                catalog.Title = title.InnerText.ClearNotWords();
                catalog.IsChecked = true;

                reusltCatalogs.Add(catalog);

                var atricles = day.CssSelect("div.postTitle");
                foreach (var atricle in atricles)
                {
                    var article = new Article();
                    article.Title = atricle.InnerText.ClearNotWords();
                    var articleTitleEl = atricle.CssSelect("a.postTitle2");
                    article.URL = articleTitleEl.First().Attributes["href"].Value;
                    catalog.Articles.Add(article);
                }
            }

            return reusltCatalogs;
        }

        public override List<Article> ParseArticles(string articleUrl)
        {
            throw new NotImplementedException();
        }
    }
}