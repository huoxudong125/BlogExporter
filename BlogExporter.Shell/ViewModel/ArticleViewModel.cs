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
        private string _title;
        private string _id;
        private string _content;
        private string _publishTimeStamp;
        private bool _isChecked;
        /// <summary>
        /// Initializes a new instance of the ArchiveViewModel class.
        /// </summary>
        public ArticleViewModel()
        {
        }

        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        public string Id
        {
            get { return _id; }
            set { Set(() => Id, ref _id, value); }
        }

        public string Content
        {
            get { return _content; }
            set { Set(() => Content, ref _content, value); }
        }

        public string PublishTimeStamp
        {
            get { return _publishTimeStamp; }
            set { Set(() => PublishTimeStamp, ref _publishTimeStamp, value); }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set(() => IsChecked, ref _isChecked, value); }
        }

    }
}