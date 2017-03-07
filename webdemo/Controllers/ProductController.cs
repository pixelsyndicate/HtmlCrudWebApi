using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using webdemo.DAL;
using webdemo.Models;
using System.Web.Http.ModelBinding;
namespace webdemo.Controllers
{
    //[EnableCors(origins: "http://localhost:62271", headers: "*", methods: "*")]
    /// <summary>
    /// 
    /// </summary>
    public class ProductController : ApiController
    {
        private readonly ProductRepository _repo = new ProductRepository(new EfProducts());

        /// <summary>
        /// This will hold a serializable way to display serverside messages.
        /// </summary>
        public ModelStateDictionary ValidationErrors { get; set; }

        // GET: api/Product
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IList<Product>))]
        public IHttpActionResult Get()
        {
            IHttpActionResult result = null;
            var fromDb = _repo.GetAll().Select(p => new Product
            {
                ProductId = p.ProductId,
                IntroductionDate = p.IntroductionDate.Value,
                ProductName = p.ProductName,
                Price = p.Price.Value,
                Url = p.Url,
                Summary = p.Summary
            }).ToList();

            if (fromDb.Count > 0)
            {
                result = Ok(fromDb);
            }
            else
            {
                result = NotFound();
            }

            return result;
        }

        // GET: api/Product/5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
                prod.Summary = ent.Summary;
                toReturn = Ok(prod);
            }
            else
            {
                toReturn = NotFound();
            }

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post(Product product)
        {
            IHttpActionResult ret = null;

            if (Validate(product))
            {
                if (_repo.Add(product))
                {
                    // get the id that we just created.
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
            }
            else
            {
                ret = BadRequest(ValidationErrors);
            }


            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult Put(int id, Product product)
        {
            IHttpActionResult ret = null;
            if (Validate(product))
            {
                if (_repo.Exists(id))
                {
                    if (_repo.Update(product))
                    {
                        ret = Ok(product);
                    }
                    else
                    {
                        ret = InternalServerError();
                    }
                }
                else
                {
                    ret = NotFound();
                }
            }
            else
            {
                ret = BadRequest(ValidationErrors);
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        protected bool Validate(Product product)
        {
            bool ret = false;

            // add custom validation
            if (product.IntroductionDate < Convert.ToDateTime("1/1/2010"))
            {
                ModelState.AddModelError("Introduction Date", "Introduction Date Must Be Greater Than 1/1/2010");
            }

            // Add more server-side validation here to match
            // or if using DataAnnotations (like with Entity Objects), you can retrieve the ModelState (Dictionary) object, 
            //get the errors from the annotations and add those to the ValidationErrors collection property.

            ValidationErrors = ModelState;
            return ModelState.IsValid;
            ret = (ValidationErrors.Count == 0);

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