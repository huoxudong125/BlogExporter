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
        public async Task Export(List<Article> articles, IBlogProcess processer
            , IProgress<DownloadStringTaskAsyncExProgress> progress = null)
        {
            foreach (var article in articles)
            {
                if (await processer.ExtractArticleContent(article, progress).ConfigureAwait(false))
                {
                    await Task.Yield();
                    await SaveToFile(article, article.Content, progress).ConfigureAwait(false);
                }
            }
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