using Blog.Common.Entities;
using GalaSoft.MvvmLight;

namespace BlogExporter.Shell.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ArticleViewModel : ViewModelBase
    {
        #region Properties

        public Article CurrentEntity { get; set; }

        #endregion Properties

        #region .octor

        public ArticleViewModel() : this(new Article())
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArchiveViewModel class.
        /// </summary>
        public ArticleViewModel(Article article)
        {
            CurrentEntity = article;
        }

        #endregion .octor
    }
}