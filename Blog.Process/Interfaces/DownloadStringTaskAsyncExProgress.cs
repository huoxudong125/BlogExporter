using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Process.Interfaces
{
    public class DownloadStringTaskAsyncExProgress
    {
        public int ProgressPercentage { get; set; }

        public string Text { get; set; }
    }
}
