using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Blog.Common;
using Blog.Common.Entities;
using Blog.Process;
using Blog.Process.Interfaces;
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
        private double _progressValue;
        private string _cnBlogName;

        #region .octor

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            LoadUrlCommand = new RelayCommand(OnLoadUrl);
            ParseUrlCommand = new RelayCommand(OnParseUrl, () => !string.IsNullOrEmpty(URL));
            ExportCommand = new RelayCommand(OnExport);
            GetUrlsCommand = new RelayCommand(OnGetUrls);

            _catalogObservableList = new ObservableCollection<CatalogNodeViewModel>();
            CatalogCollectionView = new ListCollectionView(_catalogObservableList);

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                URL = "http://hqfz.cnblgos.com";
                CnBlogName = "hqfz";
                var catalog = new CatalogNodeViewModel();
                catalog.CurrentEntity.Title = "Catalog1";
                var article = new ArticleViewModel();
                article.CurrentEntity.Title = "Article1";
                catalog.AddArticle(article);

                _catalogObservableList.Add(catalog);
            }
            else
            {
                // Code runs "for real"
                URL = "http://www.cnblogs.com/artech/default.html?page=1";
                CnBlogName = "artech";
            }
        }

    
        #endregion .octor

        #region Command

        public ICommand LoadUrlCommand { get; private set; }

        public ICommand ParseUrlCommand { get; private set; }

        public ICommand ExportCommand { get; private set; }

        public ICommand GetUrlsCommand { get; private set; }

        #endregion Command

        #region Properties

        public string CnBlogName
        {
            get { return _cnBlogName; }
            set { Set(ref _cnBlogName, value); }
        }

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

        public double ProgressValue
        {
            get { return _progressValue; }
            set { Set(() => ProgressValue, ref _progressValue, value); }
        }

        public ICollectionView CatalogCollectionView { get; private set; }

        #endregion Properties

        #region private function

        private void OnParseUrl()
        {
            _catalogObservableList.Clear();
            IBlogProcess blogProcess=new CnblogProcess();
            var catalogs = blogProcess.ParseCatalogs(CnBlogName);
            foreach (var catalog in catalogs)
            {
                var catalogViewModel=new CatalogNodeViewModel(catalog);

                foreach (var article in catalog.Articles)
                {
                    var articleViewModel=new ArticleViewModel(article);
                    catalogViewModel.AddArticle(articleViewModel);
                }

                _catalogObservableList.Add(catalogViewModel);
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
            var articles = new List<Article>();
            foreach (var catalog in _catalogObservableList)
            {
                foreach (var article in catalog.ArticlesCollectionView)
                {
                    articles.Add(((ArticleViewModel)article).CurrentEntity);
                }
            }
            var progress = new Progress<DownloadStringTaskAsyncExProgress>();
            Content = " ";
            int i = 0;
            progress.ProgressChanged += (s, e) =>
            {
                Content += e.Text + "";
                ProgressValue = (double)i++ / articles.Count();
            };

            var exporter = new WebUtilityExpoter();
            await exporter.Export(articles, progress);
        }


        private async void  OnGetUrls()
        {
            var cnblog = new CnblogProcess();
            ScrapingBrowser browser = new ScrapingBrowser();
            var htmlDocument = new HtmlDocument();
            var html=browser.DownloadString(new Uri("http://news.baidu.com"));
            htmlDocument.LoadHtml(html);
            IEnumerable<HtmlNode> links = htmlDocument.DocumentNode.Descendants("a")
                .Where(x => x.Attributes.Contains("href"));

            var progress = new Progress<DownloadStringTaskAsyncExProgress>();
            Content =" ";
            int i = 0;

            progress.ProgressChanged += (s, e) =>
            {
                Content += e.Text + Environment.NewLine;
                ProgressValue = (double)i++ / links.Count();
            };

           await OutputLinks(links,progress).ConfigureAwait(false);


        }

        private async  Task OutputLinks(IEnumerable<HtmlNode> links
            ,IProgress<DownloadStringTaskAsyncExProgress> progress )
        {

            foreach (var link in links)
            {
                var linktext = string.Format("Link href={0}, link text={1}"
                    , link.Attributes["href"].Value, link.InnerText);
                progress.Report(new DownloadStringTaskAsyncExProgress()
                {
                    Text = linktext,
                });
                await Task.Delay(50);
            }

        }

        #endregion private function
    }
}