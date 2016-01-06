using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using HTMLTableSamples.EntityClasses;

namespace HTMLTableSamples
{
  public class ProductController : ApiController
  {
    // GET api/<controller>
    [HttpGet()]
    public IHttpActionResult Get()
    {
      IHttpActionResult ret = null;
      List<Product> list = new List<Product>();

      list = CreateMockData();
      if (list.Count > 0)
      {
        ret = Ok(list);
      }
      else
      {
        ret = NotFound();
      }

      return ret;
    }

    //public IHttpActionResult GetSimple()
    //{
    //  IHttpActionResult ret = null;
    //  List<Product> list = new List<Product>();

    //  list = CreateMockData();
    //  ret = Ok(list);

    //  return ret;
    //}

    private List<Product> CreateMockData()
    {
      List<Product> ret = new List<Product>();
            
      ret.Add(new Product()
      {
        ProductId = 1,
        ProductName = "Extending Bootstrap with CSS, JavaScript and jQuery",
        IntroductionDate = Convert.ToDateTime("6/11/2015"),
        Url = "http://bit.ly/1SNzc0i"
      });

      ret.Add(new Product()
      {
        ProductId = 2,
        ProductName = "Build your own Bootstrap Business Application Template in MVC",
        IntroductionDate = Convert.ToDateTime("1/29/2015"),
        Url = "http://bit.ly/1I8ZqZg"
      });

      ret.Add(new Product()
      {
        ProductId = 3,
        ProductName = "Building Mobile Web Sites Using Web Forms, Bootstrap, and HTML5",
        IntroductionDate = Convert.ToDateTime("8/28/2014"),
        Url = "http://bit.ly/1J2dcrj"
      });

      return ret;
    }

    // GET api/<controller>/5
    [HttpGet()]
    public IHttpActionResult Get(int id)
    {
      IHttpActionResult ret;
      List<Product> list = new List<Product>();
      Product prod = new Product();

      list = CreateMockData();
      prod = list.Find(p => p.ProductId == id);
      if (prod == null)
      {
        ret = NotFound();
      }
      else
      {
        ret = Ok(prod);
      }

      return ret;
    }

    // POST api/<controller>
    [HttpPost()]
    public IHttpActionResult Post(Product product)
    {
      IHttpActionResult ret = null;

      if (Add(product))
      {
        ret = Created<Product>(Request.RequestUri +
                                  product.ProductId.ToString(),
                                  product);
      }
      else
      {
        ret = NotFound();
      }

      return ret;
    }

    private bool Add(Product product)
    {
      int newId = 0;
      List<Product> list = new List<Product>();

      list = CreateMockData();
      // Get the last id
      newId = list.Max(p => p.ProductId);
      newId++;
      product.ProductId = newId;
      // Add to list
      list.Add(product);

      // TODO: Change this to false to test the NotFound()
      return true;
    }

    // PUT api/<controller>/5
    [HttpPut()]
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

    private bool Update(Product product)
    {
      return true;
    }

    // DELETE api/<controller>/5
    [HttpDelete()]
    public IHttpActionResult Delete(int id)
    {
      IHttpActionResult ret = null;

      if (DeleteProduct(id))
      {
        ret = Ok(true);
      }
      else
      {
        ret = NotFound();
      }

      return ret;
    }

    private bool DeleteProduct(int id)
    {
      return true;
    }
  }
}