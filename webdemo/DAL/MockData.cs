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
                    Summary = @"How can we design systems when we don't know what we're doing?

The most exciting engineering challenges lie on the boundary of theory and the unknown. 
Not so unknown that they're hopeless, but not enough theory to predict the results of our decisions. 
Systems at this boundary often rely on emergent behavior — high-level effects that arise indirectly from low-level interactions.

When designing at this boundary, the challenge lies not in constructing the system, but in understanding it. 
In the absence of theory, we must develop an intuition to guide our decisions. The design process is thus one of exploration and discovery.


How do we explore? If you move to a new city, you might learn the territory by walking around. Or you might peruse a map. 
But far more effective than either is both together — a street-level experience with higher-level guidance.

Likewise, the most powerful way to gain insight into a system is by moving between levels of abstraction. 
Many designers do this instinctively. But it's easy to get stuck on the ground, experiencing concrete systems with no higher-level view. 
It's also easy to get stuck in the clouds, working entirely with abstract equations or aggregate statistics.

This interactive essay presents the ladder of abstraction, a technique for thinking explicitly about these levels, 
so a designer can move among them consciously and confidently.

I believe that an essential skill of the modern system designer will be using the interactive medium to move fluidly 
around the ladder of abstraction."
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Building using web forms, bootstrap and html5",
                    IntroductionDate = Convert.ToDateTime("08/28/2015"),
                    Url = "http://bit.ly/1j2dcrj",
                    Price = 30.24,
                    Summary =
                        @"We'll start with an in-depth example — designing the control system for a simple car simulation. 
Our goal is to write a set of rules that allows the car to navigate roads, such as the one to the right.

What inputs does our system have? Let's say that our car has a very simple sensor which can only detect three states: 
the car is on the road, left of the road, or right of the road.

How do we design the rules for the car to follow? We don't understand this system well enough to predict its behavior, 
so we need to prototype. We start with the simplest possible design, and thoroughly explore it. As we develop a feel for how it works, 
we'll get ideas for improvements. We try an idea, explore again, and keep iterating. Our design and our intuition should grow side-by-side.

on the road - left of road - right of road

The simplest possible rule here might be to turn right when off-road to the left, and turn left when off-road to the right.

go straight - veer right - veer left

To the right is an algorithm which encodes this strategy."
                }
            };

            return sameProducts;
        }
    }
}