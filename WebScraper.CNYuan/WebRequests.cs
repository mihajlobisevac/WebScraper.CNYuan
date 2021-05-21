﻿using System;
using System.Net;
using WebScraper.CNYuan.Common;

namespace WebScraper.CNYuan
{
    public static class WebRequests
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
    }
}
