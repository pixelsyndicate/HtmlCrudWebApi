using System;
using System.ComponentModel.DataAnnotations;

namespace webdemo.DAL
{
    public class ProductEntity
    {
        [Key]
        public int ProductId { get; set; }
        [MaxLength(150)]
        public string ProductName { get; set; }
        public DateTime? IntroductionDate { get; set; }
        public string Url { get; set; }

        public double? Price { get; set; }
    }
}