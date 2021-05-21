using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WebScraper.CNYuan.Models;

namespace WebScraper.CNYuan.Common
{
    public static class Extensions
    {
        public static HtmlDocument GetHtmlDocument(this HttpWebResponse response)
        {
            var html = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public static HtmlDocument GetHtmlDocumentWithData(this HttpWebRequest request, string currency, int page = 1)
        {
            var formData = FormConfig.CreateFormData(currency, page);
            var data = Encoding.ASCII.GetBytes(formData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            return response.GetHtmlDocument();
        }

        public static List<Record> ToListOfRecords(this List<List<string>> listOfRows)
        {
            var records = new List<Record>();

            foreach (var row in listOfRows)
            {
                if (row.Count == 7)
                {
                    try
                    {
                        var record = new Record
                        {
                            Name = row[0],
                            BuyingRate = float.Parse(row[1]),
                            CashBuyingRate = float.Parse(row[2]),
                            SellingRate = float.Parse(row[3]),
                            CashSellingRate = float.Parse(row[4]),
                            MiddleRate = float.Parse(row[5]),
                            PubTime = Convert.ToDateTime(row[6])
                        };
                        records.Add(record);
                    }
                    catch (Exception ex)
                    {
                        Extensions.Output($"[Error] Unable to add record: {ex.Message}");
                    }
                }
            }

            return records;
        }

        public static List<List<string>> GetTableRows(this HtmlDocument htmlDocument)
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
            var currenciesHtml = htmlDocument.DocumentNode
                .Descendants("select")
                .Where(node => node.GetAttributeValue("name", "").Equals("pjname"))
                .ToList();

            return currenciesHtml[0].Descendants("option")
                .Where(node => !node.GetAttributeValue("value", "").Equals("0"))
                .Select(x => x.InnerHtml)
                .ToList();
        }

        public static void Output(string message)
        {
            Console.WriteLine(message);
        }
    }
}
