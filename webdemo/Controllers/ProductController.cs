using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using AutoMapper;
using webdemo.DAL;
using webdemo.Models;
#pragma warning disable 1591

namespace webdemo.Controllers
{
    //[EnableCors(origins: "http://localhost:62271", headers: "*", methods: "*")]
    /// <summary>
    /// </summary>
    public class ProductController : ApiController
    {
        // private readonly ProductRepository _repo = new ProductRepository(new EfProducts());
        //private readonly GenericRepository<ProductEntity> _repo = new GenericRepository<ProductEntity>(new EfProducts());
        private readonly UnitOfWork unitOfWork = new UnitOfWork();

        private IMapper _mapper;

        public ProductController()
        {
            // Configure AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Product, ProductEntity>().ReverseMap();
            });
            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }


        // GET: api/Product
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IList<Product>))]
        public IHttpActionResult Get()
        {
            IHttpActionResult result;

            var fromDb = unitOfWork.ProductRepository.Get();
            var productEntities = fromDb as ProductEntity[] ?? fromDb.ToArray();
            if (productEntities.Any())
                result = Ok(_mapper.Map<List<Product>>(productEntities));
            else
                result = NotFound();

            return result;
        }

        // GET: api/Product/5
        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult Get(int id)
        {
            IHttpActionResult ret;
            ProductEntity fromDb;
            using (var uow = unitOfWork)
            {
                fromDb = unitOfWork.ProductRepository.GetByID(id);
                if (fromDb != null)
                    ret = Ok(_mapper.Map<Product>(fromDb));
                else
                    ret = NotFound();
            }
            return ret;
        }

        /// <summary>
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult Post(Product product)
        {
            IHttpActionResult ret = null;
            try
            {
                //  product =  _repo.Insert().Add(product);
                var e = _mapper.Map<ProductEntity>(product);
                using (var uow = unitOfWork)
                {
                    uow.ProductRepository.Insert(e);
                    uow.Save();
                }
                var p = _mapper.Map<Product>(e);
                ret = Created(Request.RequestUri + p.ProductId.ToString(), p);
            }
            catch (DbEntityValidationException ex)
            {
                ret = BadRequest(ValidationErrorsToMessages(ex));
            }
            catch (Exception ex)
            {
                ret = InternalServerError(ex);
            }

            return ret;
        }


        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult Put(int id, Product product)
        {
            IHttpActionResult ret;
            try
            {
                // remove pre-check because it locks the record
                // var e = unitOfWork.ProductRepository.GetByID(id);
                //  if (e != null) {
                var toSave = _mapper.Map<ProductEntity>(product);
                unitOfWork.ProductRepository.Update(toSave);
                unitOfWork.Save();
                ret = StatusCode(HttpStatusCode.Accepted);// Ok(p);
                // }
                // else
                //    ret = NotFound();
            }
            catch (DbEntityValidationException ex)
            {
                ret = BadRequest(ValidationErrorsToMessages(ex));
            }
            catch (Exception ex)
            {
                ret = InternalServerError(ex);
            }
            return ret;
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            IHttpActionResult ret;
            try
            {
                var e = unitOfWork.ProductRepository.GetByID(id);
                if (e != null)
                {
                    unitOfWork.ProductRepository.Delete(id);
                    unitOfWork.Save();
                    ret = Ok(id);
                }
                else
                    ret = NotFound();
            }
            catch (Exception ex)
            {
                ret = InternalServerError(ex);
            }

            return ret;
        }

        /// <summary>
        ///     pass DbEntityValidationException in and get back ModelState
        /// </summary>
        /// <param name="ex">DbEntityValidationException</param>
        /// <returns>ModelStateDictionary</returns>
        protected ModelStateDictionary ValidationErrorsToMessages(DbEntityValidationException ex)
        {
            var ret = new ModelStateDictionary();

            foreach (var result in ex.EntityValidationErrors)
                foreach (var item in result.ValidationErrors)
                    ret.AddModelError(item.PropertyName, item.ErrorMessage);

            return ret;
        }

        // <summary>
        // the Validate() method should automatically be called because i'm overriding the DbContext instance
        // </summary>
        // <param name="product"></param>
        // <returns></returns>
        //protected bool Validate(Product product)
        //{
        //    bool ret = false;

        //    // add custom validation
        //    if (product.IntroductionDate < Convert.ToDateTime("1/1/2010"))
        //    {
        //        ModelState.AddModelError("Introduction Date", "Introduction Date Must Be Greater Than 1/1/2010");
        //    }

        //    // Add more server-side validation here to match

        //    if (string.IsNullOrWhiteSpace(product.ProductName))
        //    {
        //        ModelState.AddModelError("ProductName", "Product Name must be filled in.");
        //    }

        //    if (string.IsNullOrWhiteSpace(product.Url))
        //    {
        //        ModelState.AddModelError("Url", "Url must be filled in.");
        //    }

        //    // or if using DataAnnotations (like with Entity Objects), you can retrieve the ModelState (Dictionary) object, 
        //    // get the errors from the annotations and add those to the ValidationErrors collection property.

        //    ValidationErrors = ModelState;
        //    return ModelState.IsValid;
        //    ret = (ValidationErrors.Count == 0);

        //    return ret;
        //}
    }
}