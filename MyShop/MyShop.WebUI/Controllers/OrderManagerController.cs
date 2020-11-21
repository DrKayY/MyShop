using MyShop.Core.Contracts;
using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyShop.WebUI.Controllers
{
    public class OrderManagerController : Controller
    {
        IOrderService orderService;

        public OrderManagerController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        // GET: OrderManager
        public ActionResult Index()
        {
            var orders = orderService.GetOrders();
            return View(orders);
        }

        public ActionResult UpdateOrder(string id)
        {
            var order = orderService.GetOrder(id);

            ViewBag.StatusList = new List<string>() { 
                "Order Created",
                "Payment Posted",
                "Order Shipped",
                "Order Received"
            };

            return View(order);
        }

        [HttpPost]
        public ActionResult UpdateOrder(Order updatedOrder, string id)
        {
            var order = orderService.GetOrder(id);

            order.OrderStatus = updatedOrder.OrderStatus;
            orderService.UpdateOrder(order);

            return RedirectToAction("Index");
        }
    }
}
