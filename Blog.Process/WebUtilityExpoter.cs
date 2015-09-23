using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Blog.Common;
using Blog.Common.Entities;
using Blog.Process.Interfaces;

namespace Blog.Process
{
    public class WebUtilityExpoter : IExporter
    {
        public string BaseFolder { get; private set; }

        public async Task Export(string fileName, List<Article> articles, IBlogProcess processer
            , IProgress<DownloadStringTaskAsyncExProgress> progress = null)
        {
            Init(fileName);
            StringBuilder sb = new StringBuilder();
            sb.Append("<ol>");
            foreach (var article in articles)
            {
                if (await processer.ExtractArticleContent(article, progress).ConfigureAwait(false))
                {
                    await Task.Yield();
                    await SaveArticleToFile(article, article.Content, progress).ConfigureAwait(false);
                }
                var articleFileName = article.Title.ToValidFileName() + ".htm";
                sb.AppendFormat("<li><a href='{0}'>{1}</a></li>"
                  , articleFileName, article.Title);
            }
            sb.Append("</ol>");
            string content2 = htmlString.Replace("{0}", fileName).Replace("\n{1}", sb.ToString());
            await SaveToFileAsync(content2, Path.Combine(BaseFolder, "_index.htm"));
        }

        private async Task SaveArticleToFile(Article article
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

            var filePath = Path.Combine(BaseFolder, article.Title.ToValidFileName() + ".htm");

            await SaveToFileAsync(content, filePath);
        }

        private void Init(string fileName)
        {
            BaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Temp\" + fileName);
            if (!Directory.Exists(BaseFolder))
            {
                Directory.CreateDirectory(BaseFolder);
            }
        }

        private static async Task SaveToFileAsync(string content, string filePath)
        {
            //TODO Change to dynamic get the encoding for page.
            using (var file = new StreamWriter(filePath, false, Encoding.GetEncoding("UTF-8")))
            {
                await file.WriteAsync(content);
                file.Close();
            }
        }

        #region string

        private string htmlString = @"<!doctype html>
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

        #endregion string
    }
}