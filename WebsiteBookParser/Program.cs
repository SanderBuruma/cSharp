using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using HtmlAgilityPack;
using System.IO;
using Epub;

namespace WebsiteBookParser
{
    class Program
    {
        static void Main()
        {
            GetHtmlAsync();

            Console.ReadLine();
        }

        private static async void GetHtmlAsync()
        {

            string url = @"http://www.religiousbookshelf.com/meditations-and-readings/";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var readingLinks = new List<string>() { };
            var readingsList = htmlDocument.DocumentNode.Descendants("ul");
            foreach (var child in readingsList.First().ChildNodes)
            {
                if (child.ChildNodes.Count == 1)
                {
                    readingLinks.Add("/" + child.FirstChild.GetAttributeValue("href", "#ERROR"));
                }
            }

            var weekLinkList = new List<string>() { };
            foreach (var reading in readingLinks)
            {
                html = await httpClient.GetStringAsync(url + reading);
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                Console.WriteLine("reading: " + reading);

                var thisWeekList = htmlDocument.DocumentNode.Descendants("ul");
                foreach (var htmlAnchor in thisWeekList.First().Descendants("a"))
                {
                    var temp = htmlAnchor.GetAttributeValue("href", "#ERROR");
                    if (temp == "#ERROR") Console.WriteLine("Error, link not found on anchor element, line: " + htmlAnchor.Line);
                    weekLinkList.Add(temp);
                }
            }

            //var file = new Epub.Document();
            //file.AddAuthor("st Alphonsus Ligouri");
            //file.AddDescription("A collection of meditations and readings by a great Saint and Doctor of the church");
            //file.AddTitle("Meditations and Readings of Saint Alphonsus");

            string wholeBookString = "";
            foreach (var link in weekLinkList)
            {

                try
                {
                    html = await httpClient.GetStringAsync(@"http://www.religiousbookshelf.com" + link);
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    var pageContentDiv = htmlDocument.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "") == "content").First();
                    wholeBookString += pageContentDiv.InnerHtml;
                }
                catch (Exception e)
                {
                    Console.WriteLine("exception:" + link);
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }
            }

            File.WriteAllText(@"C:\Users\Cubit32\Desktop\temp\meditationsandreadingsstalphonsus.html", wholeBookString);
        }
    }
}
