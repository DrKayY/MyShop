using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        ObjectCache cache = MemoryCache.Default;
        List<ProductCategory> productCategories;

        public ProductCategoryRepository()
        {
            productCategories = cache["productCategories"] as List<ProductCategory>;
            if (productCategories == null)
            {
                productCategories = new List<ProductCategory>();
            }
        }

        public void Commit()
        {
            cache["productCategories"] = productCategories;
        }

        public ProductCategory Find(string id)
        {
            var productCategory = productCategories.FirstOrDefault(p => p.Id == id);
            
            if (productCategory != null)
            {
                return productCategory;
            }
            else
            {
                throw new Exception("Product category not found");
            }
        }

        public void Insert(ProductCategory productCategory)
        {
            productCategories.Add(productCategory);
        }

        public void Update(ProductCategory productCategory, string id)
        {
            var productCategoryToUpdate = productCategories.FirstOrDefault(p => p.Id == id);

            if (productCategoryToUpdate != null)
            {
                productCategoryToUpdate.Name = productCategory.Name;
            }
            else
            {
                throw new Exception("Product category not found");
            }
        }

        public void Delete(string id)
        {
            var productCategory = productCategories.FirstOrDefault(p => p.Id == id);

            if (productCategory != null)
            {
                productCategories.Remove(productCategory);
            }
            else
            {
                throw new Exception("Product category not found");
            }
        }

        public IQueryable<ProductCategory> Collection()
        {
            return productCategories.AsQueryable();
        }
    }
}
