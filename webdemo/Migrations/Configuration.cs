using webdemo.DAL;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
namespace webdemo.Migrations
{


    internal sealed class Configuration : DbMigrationsConfiguration<webdemo.DAL.EfProducts>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "webdemo.DAL.EfProducts";
        }

        protected override void Seed(webdemo.DAL.EfProducts context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //


            // fill the database with some default data when first created.
            context.Products.AddOrUpdate(new ProductEntity
            {
                ProductId = 1,
                ProductName = "Extending bootstrap with css, JS and JQuery",
                IntroductionDate = Convert.ToDateTime("06/11/2015"),
                Url = "http://bit.ly/1I8ZqZg",
                Price = 25.98
            });
            context.Products.AddOrUpdate(new ProductEntity
            {
                ProductId = 2,
                ProductName = "Build your own Bootstrap Business",
                IntroductionDate = Convert.ToDateTime("01/29/2015"),
                Url = "http://bit.ly/1SNzC0i",
                Price = 15.49
            });
            context.Products.AddOrUpdate(new ProductEntity
            {
                ProductId = 3,
                ProductName = "Building using web forms, bootstrap and html5",
                IntroductionDate = Convert.ToDateTime("08/28/2015"),
                Url = "http://bit.ly/1j2dcrj",
                Price = 30.24
            });

            context.SaveChanges();
        }
    }
}
