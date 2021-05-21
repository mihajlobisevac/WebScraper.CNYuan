using System;

namespace WebScraper.CNYuan.Common
{
    public static class FormConfig
    {
        public static string StartDate => $"{DateTime.Now.AddDays(-2):yyyy-MM-dd}";
        public static string EndDate => $"{DateTime.Now:yyyy-MM-dd}";

        public static string CreateFormData(string currency, int page)
        {
            return $"erectDate={StartDate}&nothing={EndDate}&pjname={currency}&page={page}";
        }
    }
}
