using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> context;

        public ProductCategoryManagerController(IRepository<ProductCategory> productCategoryContext)
        {
            context = productCategoryContext;
        }

        // GET: ProductCategoryManager
        public ActionResult ProductCategory()
        {
            var productCategories = context.Collection().ToList<ProductCategory>();
            return View(productCategories);
        }

        public ActionResult CreateProductCategory()
        {
            var productCategory = new ProductCategory();
            return View(productCategory);
        }

        [HttpPost]
        public ActionResult CreateProductCategory(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }

            context.Insert(productCategory);
            context.Commit();
            
            return RedirectToAction("ProductCategory");
        }

        public ActionResult EditProductCategory(string id)
        {
            var productCategoryToEdit = context.Find(id);

            if (productCategoryToEdit == null)
            {
                return HttpNotFound();
            }

            return View(productCategoryToEdit);
        }

        [HttpPost]
        public ActionResult EditProductCategory(ProductCategory productCategory, string id)
        {
            var productCategoryToEdit = context.Find(id);

            if(productCategoryToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(productCategory);
                }

                productCategoryToEdit.Name = productCategory.Name;
                context.Commit();

                return RedirectToAction("ProductCategory");
            }
        }

        public ActionResult DeleteProductCategory(string id)
        {
            var productCategory = context.Find(id);

            if (productCategory == null)
            {
                return HttpNotFound();
            }

            return View(productCategory);
        }

        [HttpPost]
        [ActionName("DeleteProductCategory")]
        public ActionResult ConfirmDeleteProductCategory(string id)
        {
            var productCategory = context.Find(id);

            if (productCategory == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(productCategory);
                }

                context.Delete(id);
                context.Commit();

                return RedirectToAction("ProductCategory");
            }
        }
    }
}