using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Common;
using Blog.Process.Interfaces;

namespace Blog.Process
{
    public class WebUtilityExpoter : IExporter
    {
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

        private Regex reg_con = new Regex(@"<div id=""cnblogs_post_body"">([\s\S]+)</div><div id=""MySignature"">", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public async Task Export(List<Article> urls)
        {
            await Export(urls, null);
        }

        public async Task Export(List<Article> articles
            , IProgress<DownloadStringTaskAsyncExProgress> progress = null)
        {
            var web = new WebUtility();

            foreach (var article in articles)
            {
                var content = await LoadContent(web, article, progress).ConfigureAwait(false);
                await Task.Yield();
                await SaveToFile(article, content, progress).ConfigureAwait(false);
            }
        }

        private async Task<string> LoadContent(WebUtility web, Article article
            , IProgress<DownloadStringTaskAsyncExProgress> progress)
        {
            if (progress != null)
            {
                progress.Report(new DownloadStringTaskAsyncExProgress()
                    {
                        Text = string.Format("{0} [Load] {1} ({2}) {0}"
                        , Environment.NewLine, article.Title
                        , article.URL)
                    });
            }

            web.URL = article.URL;
            string content = await Task.Run(() => web.Get()).ConfigureAwait(false);
            Match mat = reg_con.Match(content);
            if (mat.Success)
            {
                content = mat.Groups[1].Value.Trim();
                return htmlString.Replace("{0}", article.Title).Replace("\n{1}", content); ;
            }
            return content;
        }

        private static async Task SaveToFile(Article article
            , string content, IProgress<DownloadStringTaskAsyncExProgress> progress)
        {
            if (progress != null)
            {
                progress.Report(new DownloadStringTaskAsyncExProgress()
                    {
                        Text = string.Format("{0} [Save] {1} ({2}) {0}"
                        , Environment.NewLine
                        , article.Title
                        , article.URL)
                    });
            }

            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = Path.Combine(folder, article.Title.ToValidFileName() + ".htm");

            //TODO Change to dynamic get the encoding for page.
            using (var file = new StreamWriter(filePath, false, Encoding.GetEncoding("UTF-8")))
            {
                await file.WriteAsync(content);
                file.Close();
            }
        }
    }
}