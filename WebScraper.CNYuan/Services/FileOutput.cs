using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void CreateRecordFile(List<Record> records)
        {
            if (records.Count < 1) return;

            var currency = records.FirstOrDefault().Name;

            var fileName = $"{currency}_{FormConfig.StartDate}_{FormConfig.EndDate}";

            var path = _configuration.GetSection("FilePath").Value;
            var ext = _configuration.GetSection("FileExtension").Value;

            var fullpath = path + fileName + ext;


            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            try
            {
                CreateTextFile(fullpath, records);
            }
            catch (Exception)
            {
                GeneralExtensions.Output($"[!] Unable to create file for {currency} records");
                throw;
            }
            finally
            {
                GeneralExtensions.Output($"[!] Successfully created file for {currency} records");
            }
        }

        static void CreateTextFile(string filePath, List<Record> records)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            using StreamWriter sw = File.CreateText(filePath);

            sw.WriteLine("Currency Name, Buying Rate, Cash Buying Rate, Selling Rate, Cash Selling Rate, Middle Rate, Pub Time" + Environment.NewLine);

            foreach (var r in records)
            {
                sw.WriteLine($"{r.Name}, {r.BuyingRate}, {r.CashBuyingRate}, {r.SellingRate}, {r.CashSellingRate}, {r.MiddleRate}, {r.PubTime}");
            }
        }
    }
}
