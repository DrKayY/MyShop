using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class HomeController : Controller
    {
        IRepository<Product> productRepo;
        IRepository<ProductCategory> productCategoryRepo;
        public HomeController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            productRepo = productContext;
            productCategoryRepo = productCategoryContext;
        }

        public ActionResult Index(string category = null)
        {
            var products = new List<Product>();
            var productList = new ProductListViewModel();

            if (category == null)
                productList.Products = productRepo.Collection();
            else
                productList.Products = productRepo.Collection().Where(p => p.Category == category).ToList();

            productList.ProductCategories = productCategoryRepo.Collection();
            return View(productList);
        }

        public ActionResult Details(string id)
        {
            var product = productRepo.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}