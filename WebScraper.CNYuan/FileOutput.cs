using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebScraper.CNYuan.Common;
using WebScraper.CNYuan.Models;

namespace WebScraper.CNYuan
{
    public class FileOutput
    {
        private readonly IConfiguration _configuration;

        public FileOutput(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Create(List<Record> records)
        {
            if (records.Count < 1) return;

            var currency = records.FirstOrDefault().Name;

            var fileName = $"{currency}_{FormConfig.StartDate}_{FormConfig.EndDate}";

            var path = _configuration.GetSection("FilePath").Value;
            var ext = _configuration.GetSection("FileExtension").Value;

            var fullpath = path + fileName + ext;

            try
            {
                if (File.Exists(fullpath))
                {
                    File.SetAttributes(fullpath, FileAttributes.Normal);
                    File.Delete(fullpath);
                }

                using (FileStream fs = File.Create(fullpath))
                {
                    byte[] header = new UTF8Encoding(true)
                        .GetBytes("Currency Name, Buying Rate, Cash Buying Rate, Selling Rate, Cash Selling Rate, Middle Rate, Pub Time"
                            + Environment.NewLine + Environment.NewLine);

                    fs.Write(header);

                    foreach (var r in records)
                    {
                        byte[] line = new UTF8Encoding(true)
                            .GetBytes($"{r.Name}, {r.BuyingRate}, {r.CashBuyingRate}, {r.SellingRate}, {r.CashSellingRate}, {r.MiddleRate}, {r.PubTime}"
                                + Environment.NewLine);

                        fs.Write(line);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Extensions.Output($"[!] Successfully created file for {currency} records");
            }
        }
    }
}
