using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using webdemo.Models;

namespace webdemo.DAL
{
    public class DemoProductDatabaseInitializer : CreateDatabaseIfNotExists<EfProducts>
    {
        protected override void Seed(EfProducts context)
        {
            base.Seed(context);
            // fill the database with some default data when first created.


            List<Product> sameProducts = MockData.CreateMockData();
            sameProducts.ForEach(a => context.Products.AddOrUpdate(new ProductEntity
            {
                IntroductionDate = a.IntroductionDate,
                Price = a.Price,
                ProductId = a.ProductId,
                ProductName = a.ProductName,
                Url = a.Url,
                Summary = a.Summary
            }));
            context.SaveChanges();
        }
    }
}