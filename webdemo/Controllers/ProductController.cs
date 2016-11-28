using System;
using System.Linq;
using System.Web.Http;
using webdemo.DAL;
using webdemo.Models;
using System.Web.Http.ModelBinding;
namespace webdemo.Controllers
{
    //[EnableCors(origins: "http://localhost:62271", headers: "*", methods: "*")]
    public class ProductController : ApiController
    {
        private readonly ProductRepository _repo = new ProductRepository(new EfProducts());

        /// <summary>
        /// This will hold a serializable way to display serverside messages.
        /// </summary>
        public ModelStateDictionary ValidationErrors { get; set; }

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

        protected bool Validate(Product product)
        {
            bool ret = false;

            ValidationErrors = new ModelStateDictionary();

            // add custom validation
            if (product.IntroductionDate < Convert.ToDateTime("1/1/2010"))
            {
                ValidationErrors.AddModelError("Introduction Date", "Introduction Date Must Be Greater Than 1/1/2010");
            }

            // Add more server-side validation here to match
            // or if using DataAnnotations (like with Entity Objects), you can retrieve the ModelStateDictionary object, 
            //get the errors from the annotations and add those to the ValidationErrors collection property.


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