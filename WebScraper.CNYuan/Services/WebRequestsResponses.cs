using HtmlAgilityPack;
using System;
using System.Net;
using System.IO;
using System.Text;
using WebScraper.CNYuan.Common;

namespace WebScraper.CNYuan
{
    public static class WebRequestsResponses
    {
        public static HttpWebResponse GetInitialResponse()
        {
            HttpWebRequest request = CreateWebRequest(UrlConstants.BankOfChina);
            return (HttpWebResponse)request.GetResponse();
        }

        public static HttpWebRequest CreateWebRequest(string url)
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

        public static HtmlDocument GetHtmlDocument(this HttpWebResponse response)
        {
            var html = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        public static HtmlDocument GetHtmlDocumentWithCurrencyData(this HttpWebRequest request, string currency, int page = 1)
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
    }
}
