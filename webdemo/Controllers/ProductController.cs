using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using webdemo.DAL;
using webdemo.Models;

namespace webdemo.Controllers
{
    //[EnableCors(origins: "http://localhost:62271", headers: "*", methods: "*")]
    public class ProductController : ApiController
    {
        private ProductRepository _repo = new ProductRepository(new EfProducts());


        // GET: api/Product
        [HttpGet]
        public IHttpActionResult Get()
        {
            IHttpActionResult toReturn = null;


            // retry
            var list = (from p in _repo.GetAll()
                        select new Product
                        {
                            ProductId = p.ProductId,
                            IntroductionDate = p.IntroductionDate.Value,
                            ProductName = p.ProductName,
                            Price = p.Price.Value,
                            Url = p.Url
                        }).ToList();

            if (list.Count > 0)
            {
                toReturn = Ok(list);
            }
            else
            {
                toReturn = NotFound();
            }

            return toReturn;
        }

        // GET: api/Product/5
        [HttpGet()]
        public IHttpActionResult Get(int id)
        {
            IHttpActionResult toReturn = null;

            Product prod = new Product();

            var ent = _repo.GetAll().FirstOrDefault(x => x.ProductId == id);

            if (ent != null)
            {
                prod.Price = ent.Price.Value;
                prod.IntroductionDate = ent.IntroductionDate.Value;
                prod.Url = ent.Url;
                prod.ProductName = ent.ProductName;
                prod.ProductId = ent.ProductId;
                toReturn = Ok(prod);
            }
            else
            {
                toReturn = NotFound();
            }

            return toReturn;
        }

        [HttpPost]
        public IHttpActionResult Post(Product product)
        {
            IHttpActionResult ret = null;

            if (_repo.Add(product))
            {
                var newRecId =
                    _repo.GetAll()
                        .First(x =>
                        x.ProductName == product.ProductName
                        && x.Price == product.Price &&
                        x.IntroductionDate == product.IntroductionDate
                        && x.Url == product.Url).ProductId;

                product.ProductId = newRecId;
                ret = Created<Product>(Request.RequestUri + product.ProductId.ToString(), product);
            }
            else
            {
                ret = InternalServerError();
            }

            return ret;
        }

        [HttpPut]
        public IHttpActionResult Put(int id, Product product)
        {
            IHttpActionResult ret = null;
            if (_repo.Exists(id))
            {
                if (_repo.Update(product))
                {
                    ret = Ok(product);
                }
                else
                {
                    ret = NotFound();
                }
            }
            else { ret = NotFound(); }

            return ret;
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            IHttpActionResult ret = null;
            if (_repo.Exists(id))
            {
                if (_repo.DeleteProduct(id))
                {
                    ret = Ok(true);
                }
                else
                {
                    ret = InternalServerError();
                }
            }
            else { ret = NotFound(); }

            return ret;
        }

    }
}




//       // POST: api/Product
//       public void Post([FromBody]string value)
//       {
//       }

//       // PUT: api/Product/5
//       public void Put(int id, [FromBody]string value)
//       {
//       }

//       // DELETE: api/Product/5
//       public void Delete(int id)
//       {
//       }