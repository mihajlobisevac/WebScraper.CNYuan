using System;

namespace WebScraper.CNYuan.Models
{
    public class Record
    {
        public string Name { get; set; }
        public float BuyingRate { get; set; }
        public float CashBuyingRate { get; set; }
        public float SellingRate { get; set; }
        public float CashSellingRate { get; set; }
        public float MiddleRate { get; set; }
        public DateTime PubTime { get; set; }
    }
}
