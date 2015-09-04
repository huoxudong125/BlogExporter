using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Common;
using Blog.Process.Interfaces;

namespace Blog.Process
{
   public class WebUtilityExpoter:IExporter
    {

       public async Task Export(List<Article> urls)
        {
           await Export(urls, null);
        }


       public async Task Export(List<Article> articles
           , IProgress<DownloadStringTaskAsyncExProgress> progress=null)
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
            using (var file = new StreamWriter(filePath, false))
            {
                await file.WriteAsync(content);
                file.Close();
            }
        }

    }
}
