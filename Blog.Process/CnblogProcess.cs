using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Common;
using Blog.Common.Entities;
using Blog.Process.Interfaces;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace Blog.Process
{
    public class CnblogProcess : BlogProcessBase
    {
        private string CatalogsUrlTemplate=@"http://www.cnblogs.com/{0}/default.aspx?page={1}&onlytitle=1";
        private Regex reg_con = new Regex(@"<div id=""cnblogs_post_body"">([\s\S]+)</div><div id=""MySignature"">"
            , RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private WebUtility web;


        public CnblogProcess()
        {
            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
            web = new WebUtility();
            web.Encode=Encoding.UTF8;
        }

        public async override Task<List<Catalog>> ParseCatalogs(string blogerName)
        {
            await Task.Yield();
            _blogName = blogerName;
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


        public async Task<int> ExtractCatalogPageCount(string url)
        {
            int pageCount = 1;
            var uri = new Uri(url);
            var browser1 = new ScrapingBrowser();
            browser1.Encoding=Encoding.UTF8;
            var html1 = browser1.DownloadString(uri);
            var doc = new HtmlDocument();
            doc.LoadHtml(html1);
            var html = doc.DocumentNode;

            var pagers =await Task.Run(()=>html.CssSelect("div.pager")).ConfigureAwait(false);

            var htmlNodes = pagers as IList<HtmlNode> ?? pagers.ToList();

            if (htmlNodes.Any())
            {
                var mp = Regex.Match(htmlNodes.First().InnerText, @"共(\d+)页");
                if (mp.Success) pageCount= int.Parse(mp.Groups[1].Value);
            }
            else
            {
                var pagerLinks = html.CssSelect("div#pager>a").Select(t => t.InnerText).ToList();
                if (!int.TryParse(pagerLinks.Last(),out pageCount))
                {
                    pagerLinks.RemoveAt(pagerLinks.Count-1);
                    pagerLinks.Last(t => int.TryParse(t, out pageCount));
                    pageCount = await ExtractCatalogPageCount(string.Format(CatalogsUrlTemplate,_blogName, pageCount));
                }

            }

          
           

            return pageCount;
        }

        private async Task<List<Catalog>> ExtractCatalogsFromWebPage(string url)
        {
            List<Catalog> reusltCatalogs = new List<Catalog>();

            var uri = new Uri(url);
            var browser1 = new ScrapingBrowser();
            browser1.Encoding = Encoding.UTF8;
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
            if (!days.Any())
            {
                days = html.CssSelect("div#container >div#wrapper >div#content");
            }

            foreach (var day in days)
            {
              
                var catalog = new Catalog();
                catalog.IsChecked = true;
                var title = day.CssSelect("div.dayTitle").FirstOrDefault();
                if(title!=null)
                {                 
                catalog.Title = title.InnerText.ClearNotWords();
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
                else
                {
                    catalog.Title = "CataLog"+DateTime.Now.ToShortTimeString();

                    var atricles = day.CssSelect("div.post");
                    foreach (var atricle in atricles)
                    {
                        var article = new Article();
                      
                        var articleTitleEl = atricle.CssSelect("a.PostTitle");
                        article.Title = articleTitleEl.First().InnerText.ClearNotWords();
                        article.URL = articleTitleEl.First().Attributes["href"].Value;
                        catalog.Articles.Add(article);
                    }
                }

                reusltCatalogs.Add(catalog);
                
              
            }

            return reusltCatalogs;
        }

        public override async Task<bool> ExtractArticleContent(Article article
            ,IProgress<DownloadStringTaskAsyncExProgress> progress )
        {
            if (article.IsLoaded)
            {
                return true;
            }

            web.URL = article.URL;
            if (progress != null)
            {
                progress.Report(new DownloadStringTaskAsyncExProgress()
                {
                    Text = string.Format("{0} [Load] {1} ({2}) {0}"
                    , Environment.NewLine, article.Title
                    , article.URL)
                });
            }

            string content = await Task.Run(() => web.Get()).ConfigureAwait(false);
            Match match = reg_con.Match(content);
            if (match.Success)
            {
                content = match.Groups[1].Value.Trim();
                article.Content= htmlString.Replace("{0}", article.Title).Replace("\n{1}", content);
                article.IsLoaded=true;
            }
            return match.Success;
        }


        #region string template
        string htmlString = @"<!doctype html>
<html dir=""ltr"" lang=""zh-CN"">
<head>
<title>{0}</title>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
<style type=""text/css"">
body {font:normal 12px/24px Arial, Helvetica, sans-serif; background:#D9F0DB;}
textarea,pre {font-family:Courier; font-size:12px;}
</style>
</head>
<body>
<p><a href='_index.htm'>&lt;&lt;目录</a></p>
{1}
<p><a href='_index.htm'>&lt;&lt;目录</a></p>
</body>
</html>";
        private string _blogName;
        #endregion
    }
}