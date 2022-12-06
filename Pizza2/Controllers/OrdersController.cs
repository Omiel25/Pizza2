using Microsoft.AspNetCore.Mvc;
using Pizza2.Data;
using Pizza2.Models;

namespace Pizza2.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            if (IsWorker())
            {
                List<ItemListHolderModel<OrderViewModel, OrderItemsViewModel>> model = new List<ItemListHolderModel<OrderViewModel, OrderItemsViewModel>>();
                var orders = _context.Orders.Where(p => p.OrderConfirmed == false).Select(p => p).ToList();

                foreach (var order in orders)
                {
                    ItemListHolderModel<OrderViewModel, OrderItemsViewModel> item = new ItemListHolderModel<OrderViewModel, OrderItemsViewModel>();
                    item.ItemsB = new List<OrderItemsViewModel>();

                    item.ItemA = order;
                    item.ItemsB.AddRange(_context.OrderItems.Where(p => p.OrderId == order.Id).Select(p => p).ToList());
                    model.Add(item);

                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult ConfirmOrder()
        {
            if (IsWorker())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult RejectOrder()
        {
            if (IsWorker())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult History()
        {
            if (IsWorker())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public JsonResult GetOrders(int id)
        {
            if (IsWorker() || IsAdmin())    
            {
                //Get all orders items
                List<ItemListHolderModel<OrderViewModel, string>> model = new List<ItemListHolderModel<OrderViewModel, string>>();
                var orders = _context.Orders.Select(o => o);

                foreach(var order in orders)
                {
                    ItemListHolderModel<OrderViewModel, string> itemHolder = new ItemListHolderModel<OrderViewModel, string>();
                    
                    //GetOrdersIds
                    var orderList = _context.OrderItems.Where(oi => oi.OrderId == order.Id).Select(oi => oi.PizzaId).ToList();
                    List<string> pizzas = new List<string>();
                    foreach(var item in orderList)
                    {
                        var pizza = _context.Pizzas.Single(p => p.Id == item);
                        pizzas.Add(pizza.PizzaName);
                    }

                    itemHolder.ItemA = order;
                    itemHolder.ItemsB = pizzas;

                }

                //Needed info for searching pizzas

                
                return Json(new {data = model });
            }

            return Json(new { data = "" });
        }
    }
}
