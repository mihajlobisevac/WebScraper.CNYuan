using HtmlAgilityPack;
using System.Collections.Generic;
using System.Net;
using WebScraper.CNYuan.Common;
using WebScraper.CNYuan.Models;

namespace WebScraper.CNYuan.Services
{
    public static class Scrapper
    {
        public static void BeginScrapping(FileOutput fileOutput)
        {
            var htmlDocument = WebRequestsResponses
                .GetInitialResponse()
                .GetHtmlDocument();

            var currencies = htmlDocument.GetCurrencies();
            GeneralExtensions.Output($"{currencies.Count} currencies loaded...");

            foreach (var currency in currencies)
            {
                //if it takes too long to scrap, set "scrapAllPages" parameter to false
                var records = ScrapCurrencyData(currency, false);

                fileOutput.CreateRecordFile(records);
            }
        }

        public static List<Record> ScrapCurrencyData(string currency, bool scrapAllPages)
        {
            GeneralExtensions.Output($"{currency} currency record scraping in progress...");

            var records = WebRequestsResponses
                .CreateWebRequest(UrlConstants.BankOfChina)
                .GetHtmlDocumentWithCurrencyData(currency)
                .GetRecords(currency, scrapAllPages);

            GeneralExtensions.Output($"{currency} currency record scraping finished... {records.Count} records scrapped");

            return records;
        }

        public static List<Record> GetRecords(this HtmlDocument htmlDocument, string currency, bool scrapAllPages)
        {
            var listOfRows = htmlDocument.GetDataByTableRows();
            var numberOfPages = htmlDocument.GetNumberOfPages();

            if (scrapAllPages && numberOfPages > 1)
            {
                for (int pageNumber = 2; pageNumber <= numberOfPages; pageNumber++)
                {
                    htmlDocument = WebRequestsResponses
                        .CreateWebRequest(UrlConstants.BankOfChina)
                        .GetHtmlDocumentWithCurrencyData(currency, pageNumber);

                    listOfRows.AddRange(htmlDocument.GetDataByTableRows());
                }
            }

            return listOfRows.ToListOfRecords();
        }
    }
}
