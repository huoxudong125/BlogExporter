using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GalaSoft.MvvmLight;

namespace BlogExporter.Shell.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CatalogNodeViewModel : ViewModelBase
    {
        private string _id;
        private string _title;
        private string _content;
        private ObservableCollection<ArticleViewModel> _articlesObservableList;
        private bool _isChecked;

        /// <summary>
        /// Initializes a new instance of the TreeNodeViewModel class.
        /// </summary>
        public CatalogNodeViewModel()
        {
            _articlesObservableList=new ObservableCollection<ArticleViewModel>();
            ArticlesCollectionView=new ListCollectionView(_articlesObservableList);
        }

        public string Title
        {
            get { return _title; }
            set { Set(()=>Title, ref _title, value); }
        }

        public string Id
        {
            get { return _id; }
            set { Set(()=>Id, ref _id, value); }
        }

        public string Content
        {
            get { return _content; }
            set { Set(()=>Content, ref _content, value); }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set(() => IsChecked, ref _isChecked, value); }
        }

        public ICollectionView ArticlesCollectionView { get; private set; }

        internal void AddArticle(ArticleViewModel articleViewModel)
        {
           _articlesObservableList.Add(articleViewModel);
        }
    }
}