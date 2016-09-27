using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using webdemo.Models;

namespace webdemo.Controllers
{
    //[EnableCors(origins: "http://localhost:62271", headers: "*", methods: "*")]
    public class ProductController : ApiController
    {
        // GET: api/Product
        [HttpGet]
        public IHttpActionResult Get()
        {
            IHttpActionResult toReturn = null;
            List<Product> list = new List<Product>();

            list = CreateMockData();

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
            List<Product> list = new List<Product>();

            Product prod = new Product();

            list = CreateMockData();
            prod = list.Find(p => p.ProductId == id);

            if (prod == null)
            {
                toReturn = NotFound();
            }
            else
            {
                toReturn = Ok(prod);
            }

            return toReturn;
        }

        [HttpPost]
        public IHttpActionResult Post(Product product)
        {
            IHttpActionResult ret = null;

            if (Add(product))
            {
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

            if (Update(product))
            {
                ret = Ok(product);
            }
            else
            {
                ret = NotFound();
            }

            return ret;
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            IHttpActionResult ret = null;
            if (Exists(id))
            {
                if (DeleteProduct(id))
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

        private bool Exists(int id)
        {
            var currentData = CreateMockData();
            var currentRec = currentData.Find(x => x.ProductId == id);
            return currentRec != null;
        }

        // mock method to simulate delete results
        private bool DeleteProduct(int id)
        {
            var currentData = CreateMockData();
            return currentData.Remove(currentData.Find(x => x.ProductId == id));
        }

        // mock method to simulate adding a new product
        private bool Add(Product product)
        {
            int newId = 0;
            List<Product> list = new List<Product>();

            list = CreateMockData();

            newId = list.Max(p => p.ProductId);
            newId++;
            product.ProductId = newId;
            list.Add(product);

            // todo: change to 'false' to test NotFound()
            return true;
        }

        // mock method to simulate an update to a record
        private bool Update(Product product)
        {
            return true;
        }

        private List<Product> CreateMockData()
        {
            List<Product> toReturn = new List<Product>();

            toReturn.Add(new Product
            {
                ProductId = 1,
                ProductName = "Extending bootstrap with css, javascript and jquery",
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
                ProductName = "Building mobile web sites using web forms, bootstrap and html5",
                IntroductionDate = Convert.ToDateTime("08/28/2015"),
                Url = "http://bit.ly/1j2dcrj",
                Price = 30.24
            });

            return toReturn;
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