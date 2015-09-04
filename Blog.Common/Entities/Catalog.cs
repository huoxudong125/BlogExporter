using GalaSoft.MvvmLight;

namespace Blog.Common.Entities
{
    public class Catalog : ViewModelBase
    {
        private string _id;
        private string _title;
        private string _content;
        private bool _isChecked;

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

        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set(() => IsChecked, ref _isChecked, value); }
        }
    }
}