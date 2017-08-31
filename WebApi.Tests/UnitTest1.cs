using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using webdemo.Controllers;
using webdemo.Models;

namespace WebApi.Tests
{
    [TestClass]
    public class ControllerTests
    {
        //   private ITransformRepository mockRepo;

        [TestMethod]
        public void GetReturns()
        {
            // Arrange
            var controller = new ProductController
            {
                // before we used IHttpActionResult, we needed to set-up the HttpResponseMessage 
                // during the unit test... had to set Request and Configuration on the controller else 
                // test will fail with an ArgumentNullException or InvalidOperationException.
                Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Mocks/api/"),
                Configuration = new HttpConfiguration()
            };

            // Act
            var actionResult = controller.Get();
            var contentResult = actionResult as OkNegotiatedContentResult<List<Product>>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            var resultJobs = contentResult.Content;
            Assert.IsInstanceOfType(resultJobs, typeof(List<Product>));
            Assert.IsTrue(resultJobs.Any());
            Debug.WriteLine($"I found {resultJobs.Count} records. Here's the first one...");
            Debug.WriteLine($"Project Name: {contentResult.Content.First().ProductName}");
            Debug.WriteLine($"Product Price: {contentResult.Content.First().Price:C}");
        }


        [TestMethod]
        public void GetReturnsProductWithSameId()
        {
            // Arrange
            var controller = new ProductController
            {
                Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Mocks/api/")
            };

            // Act
            var actionResult = controller.Get(2);
            var contentResult = actionResult as OkNegotiatedContentResult<Product>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(2, contentResult.Content.ProductId);
        }


        [TestMethod]
        public async Task GetReturnsNotFound()
        {
            // Arrange
            var controller = new ProductController
            {
                Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Mocks/api/")
            };

            // Act
            var actionResult = controller.Get(1000);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task PostMethodValidationCatchesDate()
        {
            // Arrange
            var controller = new ProductController
            {
                Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Mocks/api/")
            };
            var testprod = new Product
            {
                IntroductionDate = DateTime.MinValue,
                Price = 29.99,
                ProductName = "UnitTest Product",
                Summary = "Just a test",
                Url = "http://www.pixeslydnicate.com"
            };
            // Act
            var actionResult = controller.Post(testprod);
            var createdResult = actionResult as InvalidModelStateResult;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsTrue(createdResult.ModelState.ContainsKey("Introduction Date"));
        }


        [TestMethod]
        public async Task PostMethodSetsLocationHeader()
        {
            // Arrange
            var controller = new ProductController
            {
                Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/Mocks/api/")
            };
            var testprod = new Product
            {
                IntroductionDate = DateTime.Now,
                Price = 29.99,
                ProductName = "UnitTest Product",
                Summary = "Just a test",
                Url = "http://www.pixeslydnicate.com"
            };
            // Act
            var actionResult = controller.Post(testprod);
            var createdResult = actionResult as CreatedNegotiatedContentResult<Product>;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsTrue(createdResult.Location.AbsoluteUri.Contains("http://localhost/Mocks/api/"));
        }

        [TestMethod]
        public void DeleteReturnsOk()
        {
            // get an existing product to edit
            var controller0 = new ProductController();
            var getRes = controller0.Get();
            var getResult = getRes as OkNegotiatedContentResult<List<Product>>;
            var theEditable = getResult.Content.Last();
            var testableId = theEditable.ProductId;
            controller0 = null;

            // Arrange
            var controller = new ProductController();

            // Act
            var actionResult = controller.Delete(testableId);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<int>),
                "If this failed, i probably dont have an ID for a record.");
        }


        [TestMethod]
        public async Task PutReturnsContentResult()
        {
            // Arrange

            // get an existing product to edit
            var controller0 = new ProductController();
            var getRes = controller0.Get();
            var getResult = getRes as OkNegotiatedContentResult<List<Product>>;
            var theEditable = getResult.Content.Last();
            var prevPrice = theEditable.Price;
            var newPrice = prevPrice += 9.95;
            theEditable.Price = newPrice;
            controller0 = null;

            // Act
            var controller1 = new ProductController();
            var actionResult = controller1.Put(theEditable.ProductId, theEditable);
            var contentResult = actionResult as StatusCodeResult;

            // Assert

            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            // Assert.IsNotNull(contentResult.Content);
            // Assert.AreEqual(theEditable.ProductId, contentResult.Content.ProductId);
            // Assert.AreEqual(newPrice, contentResult.Content.Price);

            // revisit and verify
            var controller2 = new ProductController();
            var getVRes = controller2.Get(theEditable.ProductId);
            var getVerifyResult = getVRes as OkNegotiatedContentResult<Product>;
            var theEdited = getVerifyResult.Content;
            Assert.AreEqual(newPrice, theEdited.Price);
            controller2 = null;
        }
    }
}