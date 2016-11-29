using System;

namespace webdemo.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime IntroductionDate { get; set; }
        public string Url { get; set; }

        public double Price { get; set; }

        public string Summary { get; set; }
    }
}
