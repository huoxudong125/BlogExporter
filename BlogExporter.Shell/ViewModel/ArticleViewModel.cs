using Blog.Common;
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
        public Article CurrentEntity { get; set; }

        /// <summary>
        /// Initializes a new instance of the ArchiveViewModel class.
        /// </summary>
        public ArticleViewModel()
        {
            CurrentEntity = new Article();
        }
    }
}