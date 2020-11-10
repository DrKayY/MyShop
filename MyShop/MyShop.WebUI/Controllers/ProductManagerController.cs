using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using MyShop.DataAccess.InMemory;

namespace MyShop.WebUI.Controllers
{
    public class ProductManagerController : Controller
    {
        IRepository<Product> context;
        IRepository<ProductCategory> productCategoryRepo;
        public ProductManagerController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            context = productContext;
            productCategoryRepo = productCategoryContext;
        }

        //GET: ProductManager
        public ActionResult Index()
        {
            var products = context.Collection().ToList<Product>();
            return View(products);
        }

        public ActionResult Create()
        {
            var productViewModel = new ProductManagerViewModel();

            productViewModel.Product = new Product();
            productViewModel.ProductCategories = productCategoryRepo.Collection();

            return View(productViewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                if (file != null)
                {
                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + product.Image);
                }

                context.Insert(product);
                context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string id)
        {
            var product = context.Find(id);
            
            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                var productViewModel = new ProductManagerViewModel();
                
                productViewModel.Product = product;
                productViewModel.ProductCategories = productCategoryRepo.Collection();

                return View(productViewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(Product product, string id, HttpPostedFileBase file)
        {
            var productToEdit = context.Find(id);

            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                if (file != null)
                {
                    productToEdit.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + productToEdit.Image);
                }

                productToEdit.Category = product.Category;
                productToEdit.Description = product.Description;
                productToEdit.Name = product.Name;
                productToEdit.Price = product.Price;

                context.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(string id)
        {
            var product = context.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(product);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string id)
        {
            var productToDelete = context.Find(id);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                context.Delete(id);
                context.Commit();

                return RedirectToAction("Index");
            }
        }
    }
}