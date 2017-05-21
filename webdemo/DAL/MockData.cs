using System;
using System.Collections.Generic;
using webdemo.Models;

namespace webdemo.DAL
{
    public class MockData
    {
        public static List<Product> CreateMockData()
        {
            List<Product> sameProducts = new List<Product>
            {
                new Product
                {
                    ProductId = 1,
                    ProductName = "Extending bootstrap with css, JS and JQuery",
                    IntroductionDate = Convert.ToDateTime("06/11/2015"),
                    Url = "http://bit.ly/1I8ZqZg",
                    Price = 25.98,
                    Summary = @"In science, if you know what you are doing, you should not be doing it. 
In engineering, if you do not know what you are doing, you should not be doing it. Of course, you seldom, if ever, see either pure state.
—Richard Hamming, The Art of Doing Science and Engineering"
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Build your own Bootstrap Business",
                    IntroductionDate = Convert.ToDateTime("01/29/2015"),
                    Url = "http://bit.ly/1SNzC0i",
                    Price = 15.49,
                    Summary = @"How can we design systems when we don't know what we're doing?"
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Building using web forms, bootstrap and html5",
                    IntroductionDate = Convert.ToDateTime("08/28/2015"),
                    Url = "http://bit.ly/1j2dcrj",
                    Price = 30.24,
                    Summary =
                        @"To the right is an algorithm which encodes this strategy."
                }
            };

            return sameProducts;
        }
    }
}