using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using webdemo.Models;

namespace webdemo.DAL
{
    /// <summary>
    /// </summary>
    public class ProductRepository
    {
        // todo: much of the silly transformations between entity and poco should be using automapper
        private readonly EfProducts _dbProducts;

        private readonly MapperConfiguration _config;
        private IMapper _mapper;

        /// <summary>
        /// </summary>
        /// <param name="dbProducts"></param>
        public ProductRepository(EfProducts dbProducts)
        {
            _dbProducts = dbProducts;
            // Configure AutoMapper
            _config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductEntity>().ReverseMap();
            });
            _config.AssertConfigurationIsValid();
            _mapper = _config.CreateMapper();
        }


        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(int id)
        {
            return _dbProducts.Products.Find(id) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product DeleteProduct(int id)
        {
            using (var db = new EfProducts())
            {
                var e = db.Products.Find(id);
                db.Products.Remove(e);
                db.SaveChanges();
                return _mapper.Map<Product>(e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="product"></param>
        /// <returns>true of changes were made</returns>
        public Product Add(Product product)
        {
            var e = _mapper.Map<ProductEntity>(product);
            _dbProducts.Products.Add(e);
            _dbProducts.SaveChanges();
            return _mapper.Map<Product>(e);
        }

        /// <summary>
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            var id = product.ProductId;

            using (var db = new EfProducts())
            {
                var e = db.Products.Find(id);

                if (e != null)
                {
                    e.IntroductionDate = product.IntroductionDate;
                    e.Price = product.Price;
                    e.ProductName = product.ProductName;
                    e.Url = product.Url;
                    e.Summary = product.Summary;

                    db.Entry(e).State = EntityState.Modified;
                    db.SaveChanges();

                    return _mapper.Map<Product>(e);
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ProductEntity> GetAll()
        {
            return _dbProducts.Products.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Product> Get()
        {
            return _mapper.Map<List<Product>>(_dbProducts.Products.ToList());
            var toReturn = _dbProducts.Products.Select(p =>
                 new Product
                 {
                     ProductId = p.ProductId,
                     IntroductionDate = p.IntroductionDate.Value,
                     ProductName = p.ProductName,
                     Price = p.Price.Value,
                     Url = p.Url,
                     Summary = p.Summary
                 });

            return toReturn.ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product Get(int id)
        {
            var toReturn = new Product();
            var found = _dbProducts.Products.Find(id);
            if (found != null)
            {
                toReturn.ProductId = found.ProductId;
                toReturn.IntroductionDate = found.IntroductionDate.GetValueOrDefault();
                toReturn.Price = found.Price.GetValueOrDefault();
                toReturn.ProductName = found.ProductName;
                toReturn.Summary = found.Summary;
                toReturn.Url = found.Url;
                return toReturn;
            }
            return null;
        }
    }
}
