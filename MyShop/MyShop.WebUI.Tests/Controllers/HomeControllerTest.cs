using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.WebUI;
using MyShop.WebUI.Controllers;
using MyShop.WebUI.Tests.Mocks;

namespace MyShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void CanGetProductCount()
        {
            // Arrange
            IRepository<Product> productRepo = new MockRepository<Product>();
            IRepository<ProductCategory> productCategoryRepo = new MockRepository<ProductCategory>();
            HomeController controller = new HomeController(productRepo, productCategoryRepo);

            productRepo.Insert(new Product());

            // Act
            ViewResult result = controller.Index() as ViewResult;
            var viewModel = (ProductListViewModel)result.ViewData.Model;

            // Assert
            Assert.AreEqual(1,  viewModel.Products.Count());
        }

    }
}
