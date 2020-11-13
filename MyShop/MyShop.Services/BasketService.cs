using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService
    {
        IRepository<Product> productRepo;
        IRepository<Basket> basketRepo;

        public const string basketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext)
        {
            this.productRepo = productContext;
            this.basketRepo = basketContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            var cookie = httpContext.Request.Cookies.Get(basketSessionName);
            var basket = new Basket();

            if (cookie != null)
            {
                var basketId = cookie.Value;

                if (!string.IsNullOrEmpty(basketId))
                    basket = basketRepo.Find(basketId);
                else
                {
                    if (createIfNull)
                        basket = CreateNewBasket(httpContext);
                }
            }
            else
            {
                if (createIfNull)
                basket = CreateNewBasket(httpContext);
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            var basket = new Basket();
            basketRepo.Insert(basket);
            basketRepo.Commit();

            var cookie = new HttpCookie(basketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Request.Cookies.Add(cookie);

            return basket;
        }

        public void AddtoBasket(HttpContextBase httpContext, string productId)
        {
            var basket = GetBasket(httpContext, true);
            var item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                var basketItem = new BasketItem() {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.BasketItems.Add(basketItem);
            }
            else
            {
                item.Quantity++;
            }

            basketRepo.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            var basket = GetBasket(httpContext, true);
            var itemToRemove = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (itemToRemove != null)
            {
                basket.BasketItems.Remove(itemToRemove);
                basketRepo.Commit();
            }
        }
    }

}
