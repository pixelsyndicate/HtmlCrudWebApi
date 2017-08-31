using System;

namespace webdemo.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Product
    {
#pragma warning disable 1591

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public DateTime IntroductionDate { get; set; }
        public string Url { get; set; }

        public double Price { get; set; }

        public string Summary { get; set; }
#pragma warning restore 1591
    }
}
