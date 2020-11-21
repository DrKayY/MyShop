using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderRepo;
        public OrderService(IRepository<Order> orderRepo)
        {
            this.orderRepo = orderRepo;
        }

        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems)
        {
            foreach (var item in basketItems)
            {
                baseOrder.OrderItems.Add(new OrderItem()
                {
                    ProductId = item.Id,
                    Image = item.Image,
                    Price = item.Price,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity
                });
            }

            orderRepo.Insert(baseOrder);
            orderRepo.Commit();
        }

        public List<Order> GetOrders()
        {
            var orders = orderRepo.Collection().ToList();
            return orders;
        }

        public Order GetOrder(string id)
        {
            var order = orderRepo.Find(id);
            return order;
        }

        public void UpdateOrder(Order updatedOrder)
        {
            orderRepo.Update(updatedOrder);
            orderRepo.Commit();
        }
    }
}
