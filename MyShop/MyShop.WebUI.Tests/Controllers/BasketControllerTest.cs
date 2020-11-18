using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.Services;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTest
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            //Arrange
            IRepository<Product> productRepo = new MockRepository<Product>();
            IRepository<Basket> basketRepo = new MockRepository<Basket>();
            IRepository<Order> orderRepo = new MockRepository<Order>();
            IRepository<Customer> customerRepo = new MockRepository<Customer>();

            var mockHttpContext = new MockHttpContext();
            var basketService = new BasketService(productRepo, basketRepo);
            var orderService = new OrderService(orderRepo);

            var basketController = new BasketController(basketService, orderService, customerRepo);

            basketController.ControllerContext = new ControllerContext(mockHttpContext, new System.Web.Routing.RouteData(), basketController);

            //Act
            basketController.AddToBasket("2");
            //basketService.AddtoBasket(mockHttpContext, "1");
            
            var basket = basketRepo.Collection().FirstOrDefault();

            //Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("2", basket.BasketItems.FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            //Arrange
            IRepository<Product> productRepo = new MockRepository<Product>();
            IRepository<Basket> basketRepo = new MockRepository<Basket>();
            IRepository<Order> orderRepo = new MockRepository<Order>();
            IRepository<Customer> customerRepo = new MockRepository<Customer>();

            productRepo.Insert(new Product() { Id = "1", Price = 10.00m });
            productRepo.Insert(new Product() { Id = "2", Price = 5.00m });

            var basket = new Basket();

            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 2 });

            basketRepo.Insert(basket);

            var basketService = new BasketService(productRepo, basketRepo);
            var orderService = new OrderService(orderRepo);
            var basketController = new BasketController(basketService, orderService, customerRepo);

            
            var mockHttpContext = new MockHttpContext();
            mockHttpContext.Request.Cookies.Add(new HttpCookie("eCommerceBasket") { Value = basket.Id });
            basketController.ControllerContext = new ControllerContext(mockHttpContext, new System.Web.Routing.RouteData(), basketController);

            //Act
            var result = basketController.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            //Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(4, basketSummary.BasketCount);
            Assert.AreEqual(30.00m, basketSummary.BasketTotal);
        }

        [TestMethod]
        public void CanCompleteOrderCheckout()
        {
            //Arrange
            IRepository<Product> productRepo = new MockRepository<Product>();
            IRepository<Basket> basketRepo = new MockRepository<Basket>();
            IRepository<Order> orderRepo = new MockRepository<Order>();
            IRepository<Customer> customerRepo = new MockRepository<Customer>();

            var basketService = new BasketService(productRepo, basketRepo);
            var orderService = new OrderService(orderRepo);

            var basketController = new BasketController(basketService, orderService, customerRepo);

            productRepo.Insert(new Product() { Id = "1", Price = 29.00m });
            productRepo.Insert(new Product() { Id = "2", Price = 9.00m });

            var basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });

            basketRepo.Insert(basket);
            
            customerRepo.Insert(new Customer() { Id = "1", Email = "ykbello2011@gmail.com", ZipCode = "1111" });
            var fakeUser = new GenericPrincipal(new GenericIdentity("ykbello2011@gmail.com"), null);

            var mockHttpContext = new MockHttpContext();
            mockHttpContext.User = fakeUser;
            mockHttpContext.Request.Cookies.Add(new HttpCookie("eCommerceBasket") { Value = basket.Id });

            basketController.ControllerContext = new ControllerContext(mockHttpContext, new System.Web.Routing.RouteData(), basketController);

            //Act
            var order = new Order();
            basketController.Checkout(order);

            //Assert
            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            var orderInRepo = orderRepo.Find(order.Id);
            Assert.AreEqual(2, orderInRepo.OrderItems.Count);
        }
    }
}
