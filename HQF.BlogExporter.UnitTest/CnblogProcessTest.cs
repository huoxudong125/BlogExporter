using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Process;
using Xunit;

namespace HQF.BlogExporter.UnitTest
{
    
   public class CnblogProcessTest
    {
       [Fact]
       public async void TestExtractPageCount()
       {
           var cnblogProcess=new CnblogProcess();
           var count=await cnblogProcess.ExtractCatalogPageCount(@"http://www.cnblogs.com/hqfz/default.aspx?page=2");
           Assert.Equal(5,count);

       }
    }
}
