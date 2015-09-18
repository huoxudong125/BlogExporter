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
        private ObservableCollection<ArticleViewModel> _articlesObservableList;

        #region .octor

        /// <summary>
        /// Initializes a new instance of the TreeNodeViewModel class.
        /// </summary>
        public CatalogNodeViewModel():this(new Catalog())
        {
          
        }

        public CatalogNodeViewModel(Catalog catalog)
        {
            CurrentEntity = catalog;
            _articlesObservableList = new ObservableCollection<ArticleViewModel>();
            ArticlesCollectionView = new ListCollectionView(_articlesObservableList);

        }

        #endregion .octor

        #region Properties

        public Catalog CurrentEntity { get; private set; }

        public ICollectionView ArticlesCollectionView { get; private set; }

        #endregion Properties

        internal void AddArticle(ArticleViewModel articleViewModel)
        {
            _articlesObservableList.Add(articleViewModel);
        }
    }
}