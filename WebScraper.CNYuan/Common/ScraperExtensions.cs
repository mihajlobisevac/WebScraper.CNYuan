using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebScraper.CNYuan.Common
{
    public static class ScraperExtensions
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
            var pageSizeSearch = "var m_nPageSize = ";
            var recordCountSearch = "var m_nRecordCount = ";

            var maxRecordsPerPage = GetValueFromVarDeclaration(htmlDocument.ParsedText, pageSizeSearch);
            var totalRecordCount = GetValueFromVarDeclaration(htmlDocument.ParsedText, recordCountSearch);

            var numberOfFullPages = totalRecordCount / maxRecordsPerPage;
            var moduo = totalRecordCount % maxRecordsPerPage;

            return moduo == 0 
                ? numberOfFullPages 
                : numberOfFullPages + 1;
        }

        static int GetValueFromVarDeclaration(string htmlText, string subtext)
        {
            var subtextIndex = htmlText.IndexOf(subtext);

            if (subtextIndex < 1) return 1;

            var valueStartIndex = subtextIndex + subtext.Length;
            var valueEndIndex = htmlText.IndexOf(';', valueStartIndex);

            if (valueEndIndex < 1) return 1;

            var value = htmlText[valueStartIndex..valueEndIndex];

            return Convert.ToInt32(value);
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
