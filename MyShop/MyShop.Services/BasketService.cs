using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Services
{
    public class BasketService : IBasketService
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
            httpContext.Response.Cookies.Add(cookie);

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

        public List<BasketItemViewModel> GetBasketItems(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);

            if (basket != null)
            {
                var basketItems = (from b in basket.BasketItems
                                   join p in productRepo.Collection()
                                   on b.ProductId equals p.Id
                                   select new BasketItemViewModel() {
                                    Id = b.Id,
                                    Quantity = b.Quantity,
                                    ProductName = p.Name,
                                    Image = p.Image,
                                    Price = p.Price
                                   }).ToList();

                return basketItems;
            }

            return new List<BasketItemViewModel>();
        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);
            var basketSummary = new BasketSummaryViewModel(0, 0);

            if (basket != null)
            {
                int? count = (from b in basket.BasketItems
                              select b.Quantity).Sum();
                decimal? total = (from b in basket.BasketItems
                                  join p in productRepo.Collection()
                                  on b.ProductId equals p.Id
                                  select b.Quantity * p.Price).Sum();

                basketSummary.BasketCount = count ?? 0;
                basketSummary.BasketTotal = total ?? decimal.Zero;

                return basketSummary;
            }

            return basketSummary;
        }

        public void ClearBasket(HttpContextBase httpContext)
        {
            var basket = GetBasket(httpContext, false);
            if (basket != null)
            {
                basket.BasketItems.Clear();
                basketRepo.Commit();
            }
        }
    }
}
