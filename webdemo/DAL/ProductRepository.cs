using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webdemo.Models;

namespace webdemo.DAL
{
    public class ProductRepository
    {
        private EfProducts _dbProducts;

        public ProductRepository(EfProducts dbProducts)
        {
            _dbProducts = dbProducts;
        }


        public bool Exists(int id)
        {
            var foundRec = _dbProducts.Products.Find(id);
            return foundRec != null;
        }

        // mock method to simulate delete results
        public bool DeleteProduct(int id)
        {
            using (var db = new EfProducts())
            {
                db.Products.Remove(db.Products.Find(id));
                return db.SaveChanges() >= 1;
            }
        }

        // mock method to simulate adding a new product
        public bool Add(Product product)
        {
            var toInsert = new ProductEntity()
            {
                IntroductionDate = product.IntroductionDate,
                Price = product.Price,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Url = product.Url
            };

            _dbProducts.Products.Add(toInsert);
            var changesMade = _dbProducts.SaveChanges();
            return (changesMade >= 1);
        }

        // mock method to simulate an update to a record
        public bool Update(Product product)
        {
            var id = product.ProductId;

            using (var db = new EfProducts())
            {
                var fromDb = db.Products.Find(id);

                fromDb.IntroductionDate = product.IntroductionDate;
                fromDb.Price = product.Price;
                fromDb.ProductId = id;
                fromDb.ProductName = product.ProductName;
                fromDb.Url = product.Url;

                return db.SaveChanges() >= 1;
            }
        }

        public List<ProductEntity> GetAll()
        {
            // if it's empty, lets seed it todo: lets move this to someplace else. like migrations
            if (_dbProducts.Products.ToList().Count < 1) // might need to fill in the DB
            {
                var mockData = CreateMockData();
                using (var db = new EfProducts())
                {
                    db.Products.AddRange(mockData.Select(x => new ProductEntity
                    {
                        ProductId = 0, //x.ProductId,
                        IntroductionDate = x.IntroductionDate,
                        ProductName = x.ProductName,
                        Price = x.Price,
                        Url = x.Url
                    }));
                    db.SaveChanges();
                }
            }
            return _dbProducts.Products.ToList();

        }

        public List<Product> CreateMockData()
        {
            List<Product> toReturn = new List<Product>();

            toReturn.Add(new Product
            {
                ProductId = 1,
                ProductName = "Extending bootstrap with css, JS and JQuery",
                IntroductionDate = Convert.ToDateTime("06/11/2015"),
                Url = "http://bit.ly/1I8ZqZg",
                Price = 25.98
            });

            toReturn.Add(new Product
            {
                ProductId = 2,
                ProductName = "Build your own Bootstrap Business",
                IntroductionDate = Convert.ToDateTime("01/29/2015"),
                Url = "http://bit.ly/1SNzC0i",
                Price = 15.49
            });

            toReturn.Add(new Product
            {
                ProductId = 3,
                ProductName = "Building using web forms, bootstrap and html5",
                IntroductionDate = Convert.ToDateTime("08/28/2015"),
                Url = "http://bit.ly/1j2dcrj",
                Price = 30.24
            });

            return toReturn;
        }


    }
}