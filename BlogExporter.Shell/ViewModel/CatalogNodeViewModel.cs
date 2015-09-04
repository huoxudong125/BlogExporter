using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Blog.Common.Entities;

namespace BlogExporter.Shell.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CatalogNodeViewModel
    {
        public Catalog CurrentEntity { get; private set; }

        private ObservableCollection<ArticleViewModel> _articlesObservableList;

        /// <summary>
        /// Initializes a new instance of the TreeNodeViewModel class.
        /// </summary>
        public CatalogNodeViewModel()
        {
            CurrentEntity = new Catalog();
            _articlesObservableList = new ObservableCollection<ArticleViewModel>();
            ArticlesCollectionView = new ListCollectionView(_articlesObservableList);
        }

        public ICollectionView ArticlesCollectionView { get; private set; }

        internal void AddArticle(ArticleViewModel articleViewModel)
        {
            _articlesObservableList.Add(articleViewModel);
        }
    }
}