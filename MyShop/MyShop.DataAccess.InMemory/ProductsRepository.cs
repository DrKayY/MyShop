using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class ProductsRepository
    {
        ObjectCache cache = MemoryCache.Default;
        List<Product> products;
        public ProductsRepository()
        {
            products = cache["products"] as List<Product>;
            if (products == null)
            {
                products = new List<Product>();
            }
        }

        public void Commit()
        {
            cache["products"] = products;
        }

        public void Insert(Product product)
        {
            products.Add(product);
        }

        public void Update(Product product)
        {
            var productToUpdate = products.FirstOrDefault(p => p.Id == product.Id);

            if (productToUpdate != null)
            {
                productToUpdate = product;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public Product Find(string id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                return product;
            }
            else
            {
                throw new Exception("Product not found");
            }
        }

        public IQueryable<Product> Collection()
        {
            return products.AsQueryable();
        }

        public void Delete(string id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                products.Remove(product);
            }
            else
            {
                throw new Exception("Product not found");
            }
        }
    }
}
