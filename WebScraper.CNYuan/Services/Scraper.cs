using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebScraper.CNYuan.Common;
using WebScraper.CNYuan.Models;

namespace WebScraper.CNYuan.Services
{
    public static class Scraper
    {
        public static void BeginScraping(IConfiguration configuration)
        {
            var fileOutput = new FileOutput(configuration);
            var currencies = GetCurrencies();

            foreach (var currency in currencies)
            {
                var records = ScrapeCurrencyData(currency);

                fileOutput.CreateRecordFile(records);
            }
        }

        static List<string> GetCurrencies()
        {
            var initialHttpWebResponse = WebRequestsResponses.GetInitialResponse();

            var currencies = initialHttpWebResponse
                .GetHtmlDocument()
                .GetCurrencies();

            GeneralExtensions.Output($"{currencies.Count} currencies loaded...");

            return currencies;
        }

        public static List<Record> ScrapeCurrencyData(string currency)
        {
            GeneralExtensions.Output($"{currency} currency record scraping in progress...");

            var records = WebRequestsResponses.CreateWebRequest(UrlConstants.BankOfChina)
                .GetHtmlDocumentWithCurrencyData(currency)
                .GetRecords(currency);

            GeneralExtensions.Output($"{currency} currency record scraping finished... {records.Count} records scraped");

            return records;
        }

        public static List<Record> GetRecords(this HtmlDocument htmlDocument, string currency, bool scrapeAllPages = false)
        {
            var listOfRows = htmlDocument.GetDataByTableRows();
            var numberOfPages = htmlDocument.GetNumberOfPages();

            if (scrapeAllPages && numberOfPages > 1)
            {
                ScrapeRemainingPages(listOfRows, numberOfPages, currency);
            }

            return listOfRows.ToListOfRecords();
        }

        static void ScrapeRemainingPages(List<List<string>> listOfRows, int numberOfPages, string currency)
        {
            for (int pageNumber = 2; pageNumber <= numberOfPages; pageNumber++)
            {
                GeneralExtensions.Output($"{currency} page {pageNumber} record scraping in progress...");

                var httpWebRequest = WebRequestsResponses.CreateWebRequest(UrlConstants.BankOfChina);
                var htmlDocument = httpWebRequest.GetHtmlDocumentWithCurrencyData(currency, pageNumber);

                listOfRows.AddRange(htmlDocument.GetDataByTableRows());
            }
        }
    }
}
