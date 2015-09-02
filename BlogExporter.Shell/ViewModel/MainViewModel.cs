using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Blog.Process;
using BlogExporter.Shell.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace BlogExporter.Shell.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    ///     </para>
    ///     <para>
    ///         You can also use Blend to data bind with the tool's support.
    ///     </para>
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly ObservableCollection<CatalogNodeViewModel> _catalogObservableList;
        private string _content;
        private string _url;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LoadUrlCommand = new RelayCommand(OnLoadUrl);
            ParseUrlCommand = new RelayCommand(OnParseUrl, () => !string.IsNullOrEmpty(URL));
            ExportCommand = new RelayCommand(OnExport);

            _catalogObservableList = new ObservableCollection<CatalogNodeViewModel>();
            CatalogCollectionView = new ListCollectionView(_catalogObservableList);

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                URL = "http://hqfz.cnblgos.com";
                var catalog = new CatalogNodeViewModel
                {
                    Title = "Catalog1"
                };
                catalog.AddArticle(new ArticleViewModel()
                {
                    Title = "Article1"
                });
                _catalogObservableList.Add(catalog);
            }
            else
            {
                // Code runs "for real"
                URL = "http://www.cnblogs.com/artech/default.html?page=1";
            }
        }

        public ICommand LoadUrlCommand { get; private set; }
        public ICommand ParseUrlCommand { get; private set; }

        public ICommand ExportCommand { get; private set; }

        #region Properties

        public string URL
        {
            get { return _url; }
            set { Set(() => URL, ref _url, value); }
        }

        public string Content
        {
            get { return _content; }
            set { Set(() => Content, ref _content, value); }
        }

        public ICollectionView CatalogCollectionView { get; private set; }

        #endregion Properties

        #region private function

        private void OnParseUrl()
        {
            _catalogObservableList.Clear();
            var uri = new Uri(URL);
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
                var catalog = new CatalogNodeViewModel()
                {
                    Title = title.InnerText.ClearNotWords(),
                    IsChecked = true
                };
                _catalogObservableList.Add(catalog);

                var atricles = day.CssSelect("div.postTitle");
                foreach (var atricle in atricles)
                {
                    var articleModle = new ArticleViewModel() { Title = atricle.InnerText.ClearNotWords() };
                    var articleTitleEl = atricle.CssSelect("a.postTitle2");
                    articleModle.URL = articleTitleEl.First().Attributes["href"].Value;
                    catalog.AddArticle(articleModle);
                }
            }
        }

        private void OnLoadUrl()
        {
            var uri = new Uri(URL);
            var browser1 = new ScrapingBrowser();
            var html1 = browser1.DownloadString(uri);
            var doc = new HtmlDocument();
            doc.LoadHtml(html1);
            Content = doc.DocumentNode.InnerHtml;
        }

        private async void OnExport()
        {
            var articles = new List<ArticleViewModel>();
            var web = new WebUtility();
            foreach (var catalog in _catalogObservableList)
            {
                foreach (var article in catalog.ArticlesCollectionView)
                {
                    articles.Add(article as ArticleViewModel);
                }
            }

          await  DownLoad(articles, web);
        }

        private async Task DownLoad(List<ArticleViewModel> articles, WebUtility web)
        {
            Content = string.Empty;


            var progress = new Progress<DownloadStringTaskAsyncExProgress>();
            progress.ProgressChanged += (s, e) =>
            {
                //MessageBox.Show(e.ProgressPercentage + "");
                Content += e.Text + "";
            };
            foreach (var articleViewModel in articles)
            {
                var content =await LoadContent(web, articleViewModel,progress).ConfigureAwait(false);
                await Task.Yield();
                await SaveToFile(articleViewModel, content, progress).ConfigureAwait(false);
            }
        }

        private async Task<string> LoadContent(WebUtility web, ArticleViewModel articleViewModel
            ,IProgress<DownloadStringTaskAsyncExProgress> progress)
        {
            progress.Report(new DownloadStringTaskAsyncExProgress()
            {
                Text = string.Format("{0} [Load] {1} ({2}) {0}", Environment.NewLine, articleViewModel.Title, articleViewModel.URL)
            });
       

            web.URL = articleViewModel.URL;
            string content = web.Get();
            return content;
        }

        private static async Task SaveToFile(ArticleViewModel articleViewModel
            , string content,IProgress<DownloadStringTaskAsyncExProgress> progress )
        {
            progress.Report(new DownloadStringTaskAsyncExProgress()
            {
                Text = string.Format("{0} [Save] {1} ({2}) {0}", Environment.NewLine, articleViewModel.Title, articleViewModel.URL)
            });

            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var filePath = Path.Combine(folder, articleViewModel.Title.ToValidFileName() + ".htm");
            using (var file = new StreamWriter(filePath, false))
            {
                await file.WriteAsync(content);
                file.Close();
            }
        }

        #endregion private function
    }

    public class DownloadStringTaskAsyncExProgress
    {
        public int ProgressPercentage { get; set; }
        public string Text { get; set; }
    }

}