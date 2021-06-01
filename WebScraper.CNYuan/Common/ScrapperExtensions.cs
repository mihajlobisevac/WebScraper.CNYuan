using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebScraper.CNYuan.Common
{
    public static class ScrapperExtensions
    {
        public static List<List<string>> GetDataByTableRows(this HtmlDocument htmlDocument)
        {
            var xpath = "//td[@class='hui12_20']/..";

            var inputNodes = htmlDocument.DocumentNode
                .SelectNodes(xpath)
                .Select(node => node)
                .ToList();

            var rowList = new List<List<string>>();

            foreach (var node in inputNodes)
            {
                var rowValues = node.Descendants("td")
                    .Select(child => child.InnerText)
                    .ToList();

                rowList.Add(rowValues);
            }

            return rowList;
        }

        public static int GetNumberOfPages(this HtmlDocument htmlDocument)
        {
            //max amount of pages 999(?)
            const int TRIPLE_DIGIT_MAX = 3;

            //locate where page size is declared
            var pageSizeSearch = "var m_nPageSize = ";
            var index = htmlDocument.ParsedText.IndexOf(pageSizeSearch);

            //get value after from declaration
            var offset = index + pageSizeSearch.Length;
            var numberOfPages = htmlDocument.ParsedText.Substring(offset, TRIPLE_DIGIT_MAX);

            //remove non-digit chars
            var justNumbers = new string(numberOfPages.Where(char.IsDigit).ToArray());

            if (justNumbers.Length < 1) return 1;

            var pageSize = Convert.ToInt32(justNumbers);

            return pageSize == 0 ? 1 : pageSize;
        }

        public static List<string> GetCurrencies(this HtmlDocument htmlDocument)
        {
            var htmlNode = htmlDocument.DocumentNode
                .Descendants("select")
                .Where(node => node.GetAttributeValue("name", "").Equals("pjname"))
                .FirstOrDefault();

            return htmlNode.Descendants("option")
                .Where(node => !node.GetAttributeValue("value", "").Equals("0"))
                .Select(x => x.InnerHtml)
                .ToList();
        }
    }
}
