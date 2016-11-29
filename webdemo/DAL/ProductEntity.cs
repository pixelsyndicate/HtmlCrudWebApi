using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace webdemo.DAL
{
    public class ProductEntity
    {
        [Key, Column(Order = 1), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [MaxLength(150)]
        [Required]
        public string ProductName { get; set; }

        [Required]
        public DateTime? IntroductionDate { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public double? Price { get; set; }

        //[JsonIgnore]
        [MaxLength(500)]
        public string Summary { get; set; }
    }
}