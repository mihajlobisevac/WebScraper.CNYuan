using System;
using System.Collections.Generic;
using WebScraper.CNYuan.Models;

namespace WebScraper.CNYuan.Common
{
    public static class GeneralExtensions
    {
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
                        GeneralExtensions.Output($"[Error] Unable to add record: {ex.Message}");
                    }
                }
            }

            return records;
        }

        public static void Output(string message)
        {
            Console.WriteLine(message);
        }
    }
}
