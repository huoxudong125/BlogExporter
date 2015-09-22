using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Common.Entities;
using Blog.Process.Interfaces;
using ICSharpCode.SharpZipLib.Zip;

namespace Blog.Process
{
    public class EpubExporter : IExporter
    {
        private readonly string _title = "test" + DateTime.Now.ToString("yyyyMMddhhmmss");
        private readonly List<Article> articlesList = new List<Article>();
        private readonly string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Temp_Epub\");
        private readonly Encoding encode = Encoding.GetEncoding("UTF-8"); //gb18030

        public async Task Export(List<Article> urls, IBlogProcess process
            , IProgress<DownloadStringTaskAsyncExProgress> progress)
        {
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            var artUrls = urls.Select(t => t.URL).ToArray();
            if (artUrls.Any())
            {
                Build1(process, progress);
            }

            if (progress != null)
            {
                progress.Report(new DownloadStringTaskAsyncExProgress
                {
                    ProgressPercentage = 0.5f,
                    Text = "正在编译。。。"
                });
            }

            BuildTable();
            Compile();
        }

        private async Task Build1(IBlogProcess process, IProgress<DownloadStringTaskAsyncExProgress> progress)
        {          
            var cnt = articlesList.Count;
            for (var i = cnt - 1; i >= 0; i--)
            {
                var entity = articlesList[i];
                
                if (progress != null)
                {
                    progress.Report(new DownloadStringTaskAsyncExProgress
                    {
                        Text = "获取文章 " + (cnt - i) + "/" + cnt + " " + entity.Title
                    });
                }

                var fileName = "article_" + (cnt - i) + ".html";

                if (await process.ExtractArticleContent(entity, progress))
                {
                    var content = strItem.Replace("{0}", entity.Title).Replace("\n{1}", entity.Content);
                    CreateFile(baseDir + fileName, content);
                }
            }
        }

        private void BuildTable()
        {
            var guid = Guid.NewGuid().ToString();
            var sb1 = new StringBuilder();
            var sb2 = new StringBuilder();
            var sb3 = new StringBuilder();

            if (articlesList.Count > 0)
            {
                var cnt = articlesList.Count;
                for (var i = cnt - 1; i >= 0; i--)
                {
                    if (articlesList[i].IsLoaded)
                    {
                        sb1.AppendFormat(item_1, cnt - i);
                        sb2.AppendFormat(item_2, cnt - i);
                        sb3.AppendFormat(item_3, cnt - i, articlesList[i].Title);
                    }
                }
            }

            var fileName = baseDir + "content.opf";
            var content =
                strOpf.Replace("{0}", _title)
                    .Replace("{1}", guid)
                    .Replace("{2}", sb1.ToString())
                    .Replace("{3}", sb2.ToString());
            CreateFile(fileName, content);

            fileName = baseDir + "toc.ncx";
            content = strNcx.Replace("{0}", guid).Replace("{1}", _title).Replace("{2}", sb3.ToString());
            CreateFile(fileName, content);
        }

        private void Compile()
        {
            var zip = new ZipOutputStream(File.Create(baseDir.TrimEnd('\\') + ".zip"));
            zip.SetLevel(0);

            ZipFile(zip, baseDir + "mimetype");
            File.Delete(baseDir + "mimetype");

            var di = new DirectoryInfo(baseDir.TrimEnd('\\'));
            foreach (var dir in di.GetDirectories())
            {
                ZipFile(zip, dir.FullName);
            }
            foreach (var fil in di.GetFiles())
            {
                ZipFile(zip, fil.FullName);
            }

            zip.Finish();
            zip.Close();

            File.Move(baseDir.TrimEnd('\\') + ".zip", baseDir.TrimEnd('\\') + ".epub");
            Directory.Delete(baseDir.TrimEnd('\\'), true);
        }

        #region helper

        private readonly Regex reg_w = new Regex(@"\W+", RegexOptions.Compiled);

        private string GetFileName(string title)
        {
            return reg_w.Replace(title, string.Empty);
        }

        private void CreateFile(string fileName, string content)
        {
            var fs = new FileStream(fileName, FileMode.Create);
            using (var writer = new StreamWriter(fs, encode))
            {
                writer.Write(content);
            }
            fs.Dispose();
        }

        private void ZipFile(ZipOutputStream zip, string fileName)
        {
            if (Directory.Exists(fileName))
            {
                var di = new DirectoryInfo(fileName);

                var zEntry = new ZipEntry(fileName.Replace(baseDir, "") + "\\");
                zip.PutNextEntry(zEntry);

                foreach (var dir in di.GetDirectories())
                {
                    ZipFile(zip, dir.FullName);
                }
                foreach (var fil in di.GetFiles())
                {
                    ZipFile(zip, fil.FullName);
                }
            }
            else
            {
                var fs = File.OpenRead(fileName);
                var data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                var zEntry = new ZipEntry(fileName.Replace(baseDir, ""));
                zip.PutNextEntry(zEntry);
                zip.Write(data, 0, data.Length);
                fs.Close();
            }
        }

        #endregion helper

        #region string

        private string strContainer = @"<?xml version=""1.0"" ?>
<container version=""1.0"" xmlns=""urn:oasis:names:tc:opendocument:xmlns:container"">
   <rootfiles>
      <rootfile full-path=""content.opf"" media-type=""application/oebps-package+xml""/>
   </rootfiles>
</container>";

        private readonly string strOpf = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<package xmlns=""http://www.idpf.org/2007/opf"" version=""2.0"" unique-identifier=""uuid_id"">
  <metadata xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:opf=""http://www.idpf.org/2007/opf"" xmlns:dcterms=""http://purl.org/dc/terms/"" xmlns:calibre=""http://calibre.kovidgoyal.net/2009/metadata"" xmlns:dc=""http://purl.org/dc/elements/1.1/"">
    <dc:title>{0}</dc:title>
    <dc:creator>sq_zhuyi</dc:creator>
    <dc:description>{0}</dc:description>
    <dc:language>zh-cn</dc:language>
    <dc:date></dc:date>
    <dc:contributor>BLOG. [http://hqfz.cnblogs.com]</dc:contributor>
    <dc:publisher>CSDN.NET</dc:publisher>
    <dc:identifier id=""uuid_id"" opf:scheme=""uuid"">{1}</dc:identifier>
    <dc:subject>{0}</dc:subject>
  </metadata>
  <manifest>
{2}
    <item href=""cover.png"" id=""cover"" media-type=""image/png""/>
    <item href=""stylesheet.css"" id=""css"" media-type=""text/css""/>
    <item href=""titlepage.xhtml"" id=""titlepage"" media-type=""application/xhtml+xml""/>
    <item href=""page.xhtml"" id=""page"" media-type=""application/xhtml+xml""/>
    <item href=""toc.ncx"" media-type=""application/x-dtbncx+xml"" id=""ncx""/>
  </manifest>
  <spine toc=""ncx"">
    <itemref idref=""titlepage""/>
    <itemref idref=""page""/>
{3}
    <itemref idref=""page""/>
  </spine>
  <guide>
    <reference href=""titlepage.xhtml"" type=""cover"" title=""封面""/>
    <reference href=""catalog.html"" type=""toc"" title=""目录""/>
  </guide>
</package>";

        private string strCover = "http://static.blog.csdn.net/images/cover.png";

        private string strMimetype = "application/epub+zip";

        private string strTitlepage = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""zh-CN"">
    <head>
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""/>
        <title>Cover</title>
        <style type=""text/css"" title=""override_css"">
            @page {padding: 0pt; margin:0pt}
            body { text-align: center; padding:0pt; margin: 0pt; }
            div { margin: 0pt; padding: 0pt; }
        </style>
    </head>
    <body>
        <div>
            <img src=""cover.png"" alt=""cover"" style=""height: 100%""/>
        </div>
    </body>
</html>";

        private string strStyle = @"@namespace h ""http://www.w3.org/1999/xhtml"";

body{margin:10px;font-size: 12px;}
p{text-indent:2em; line-height:150%; margin-top:0; margin-bottom:0;}
textarea,pre {font-family:Courier; font-size:12px;}
.catalog{
	margin:1pt;
	padding:0;
	text-indent:2em;
}
h1{text-align:right; margin-right:2em; font-size:1.6em; font-weight:bold;}
h2 {
    display: block;
    font-size: 1.2em;
    font-weight: bold;
    margin-bottom: 0.83em;
    margin-left: 0;
    margin-right: 0;
    margin-top: 1em;
}
.mbppagebreak {
    display: block;
    margin-bottom: 0;
    margin-left: 0;
    margin-right: 0;
    margin-top: 0;
    page-break-after: always;
}
a {color: inherit;text-decoration: inherit;cursor: default}
a[href] {color: blue;text-decoration: underline;cursor: pointer}
.italic {font-style: italic}
";

        private readonly string strNcx = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<ncx xmlns=""http://www.daisy.org/z3986/2005/ncx/"" version=""2005-1"">
  <head>
    <meta content=""{0}"" name=""dtb:uid""/>
    <meta content=""2"" name=""dtb:depth""/>
    <meta content=""BLOG.CSDN.NET [http://blog.csdn.net]"" name=""dtb:generator""/>
    <meta content=""0"" name=""dtb:totalPageCount""/>
    <meta content=""0"" name=""dtb:maxPageNumber""/>
  </head>
  <docTitle>
    <text>{1}</text>
  </docTitle>
  <docAuthor>
    <text>sq_zhuyi</text>
  </docAuthor>
  <navMap>
{2}
  </navMap>
</ncx>";

        private readonly string strItem = @"<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""zh-CN"">
<head>
<title></title>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
<link href=""stylesheet.css"" type=""text/css"" rel=""stylesheet""/>
<style type=""text/css"">
@page { margin-bottom: 5.000000pt; margin-top: 5.000000pt; }
</style>
</head>
<body>
<h2><span style=""border-bottom:1px solid"">{0}</span></h2>
{1}
<div class=""mbppagebreak""></div>
</body>
</html>
";

        private readonly string item_1 =
            @"    <item href=""article_{0}.html"" id=""article_{0}"" media-type=""application/xhtml+xml""/>\r\n";

        private readonly string item_2 = @"    <itemref idref=""article_{0}""/>\r\n";

        private readonly string item_3 = @"    <navPoint class=""chapter"" id=""article_{0}"" playOrder=""{0}"">
        <navLabel>
        <text>{1}</text>
        </navLabel>
        <content src=""article_{0}.html""/>
    </navPoint>\r\n";

        #endregion string
    }
}