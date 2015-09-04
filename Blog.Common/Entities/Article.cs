using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Blog.Common
{
    public class Article : ViewModelBase
    {
        private string _title;
        private string _id;
        private string _content;
        private string _publishTimeStamp;
        private bool _isChecked;
        private string _url;
        private bool _isLoaded;

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

        public string URL
        {
            get { return _url; }
            set { Set(() => URL, ref _url, value); }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { Set(() => IsLoaded, ref _isLoaded, value); }
        }
    }
}
