using System.ComponentModel.DataAnnotations;
using webdemo.Models;

namespace webdemo.DAL
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class EfProducts : DbContext
    {
        // Your context has been configured to use a 'EfProducts' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'webdemo.DAL.EfProducts' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'EfProducts' 
        // connection string in the application configuration file.
        public EfProducts()
            : base("name=EfProducts")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<ProductEntity> Products { get; set; }

    }

    public class ProductEntity
    {
        [Key]
        public int ProductId { get; set; }
        [MaxLength(50)]
        public string ProductName { get; set; }
        public DateTime? IntroductionDate { get; set; }
        public string Url { get; set; }

        public double? Price { get; set; }
    }
    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}