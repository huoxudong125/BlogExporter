using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Html.Forms;
using ScrapySharp.Network;
using Xunit;

namespace HQF.BlogExporter.UnitTest
{

    public class ScrapySharpTest
    {
        [Fact]
        public void TestUsingForm()
        {
            ScrapingBrowser browser = new ScrapingBrowser();

            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;

            WebPage homePage = browser.NavigateToPage(new Uri("http://www.bing.com/"));

            PageWebForm form = homePage.FindFormById("sb_form");
            form["q"] = "scrapysharp";
            form.Method = HttpVerb.Get;
            WebPage resultsPage = form.Submit();

            HtmlNode[] resultsLinks = resultsPage.Html.CssSelect("div.b_content h2 a").ToArray();

            Assert.True(resultsLinks.Any());
            //WebPage blogPage = resultsPage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();

        }

    }
}
