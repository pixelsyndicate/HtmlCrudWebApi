using System.Collections.Generic;
using System.Linq;
using webdemo.Models;

namespace webdemo.DAL
{
    public class ProductRepository
    {
        private readonly EfProducts _dbProducts;

        public ProductRepository(EfProducts dbProducts)
        {
            _dbProducts = dbProducts;
        }


        public bool Exists(int id)
        {
            var foundRec = _dbProducts.Products.Find(id);
            return foundRec != null;
        }

        public bool DeleteProduct(int id)
        {
            using (var db = new EfProducts())
            {
                db.Products.Remove(db.Products.Find(id));
                return db.SaveChanges() >= 1;
            }
        }

        public bool Add(Product product)
        {
            var toInsert = new ProductEntity()
            {
                IntroductionDate = product.IntroductionDate,
                Price = product.Price,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Url = product.Url,
                Summary = product.Summary
            };

            _dbProducts.Products.Add(toInsert);
            var changesMade = _dbProducts.SaveChanges();
            return (changesMade >= 1);
        }

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
                fromDb.Summary = product.Summary;

                return db.SaveChanges() >= 1;
            }
        }

        public List<ProductEntity> GetAll()
        {
            return _dbProducts.Products.ToList();
        }
    }
}