using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WebScraper.CNYuan.Common;
using WebScraper.CNYuan.Models;

namespace WebScraper.CNYuan
{
    class Program
    {
        static void Main(string[] args)
        {
            var response = GetInitialResponse();
            var htmlDocument = response.GetHtmlDocument();
            var currencies = htmlDocument.GetCurrencies();

            foreach (var currency in currencies)
            {
                HttpWebRequest request = CreateWebRequest(UrlConstants.BankOfChina);
                htmlDocument = request.GetHtmlDocumentWithData(currency);

                var numberOfPages = htmlDocument.GetNumberOfPages();

                var listOfRows = htmlDocument.GetTableRows();

                if (numberOfPages > 1)
                {
                    for (int pageNumber = 2; pageNumber <= numberOfPages; pageNumber++)
                    {
                        HttpWebRequest newRequest = CreateWebRequest(UrlConstants.BankOfChina);
                        htmlDocument = newRequest.GetHtmlDocumentWithData(currency, pageNumber);
                        listOfRows.AddRange(htmlDocument.GetTableRows());
                    }
                }

                var records = listOfRows.ToListOfRecords();
            }

            Console.WriteLine("Hello World!");
        }

        private static HttpWebResponse GetInitialResponse()
        {
            HttpWebRequest request = CreateWebRequest(UrlConstants.BankOfChina);
            return (HttpWebResponse)request.GetResponse();
        }

        private static HttpWebRequest CreateWebRequest(string url)
        {
            HttpWebRequest request;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (UriFormatException)
            {
                request = null;
            }

            if (request == null)
            {
                throw new ApplicationException("Invalid URL: " + url);
            }

            return request;
        }
    }
}
