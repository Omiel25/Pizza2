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
            if (IsWorker() || IsAdmin())
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
                TempData["error"] = "Unauthorized access attempt!";
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult AcceptedOrders()
        {
            if (IsWorker())
            {
                List<ItemListHolderModel<OrderViewModel, string>> model = new List<ItemListHolderModel<OrderViewModel, string>>();
                var orders = _context.Orders.Where(p => p.OrderConfirmed == true).Select(p => p).ToList();

                foreach (var order in orders)
                {
                    ItemListHolderModel<OrderViewModel, string> item = new ItemListHolderModel<OrderViewModel, string>();
                    item.ItemsB = new List<string>();
                    item.ItemA = order;

                    var orderItems = _context.OrderItems.Where(oi => oi.OrderId == order.Id).Select(oi => oi.PizzaId).ToList();
                    foreach(var pizzaId in orderItems)
                    {
                        item.ItemsB.Add(_context.Pizzas.Where(p => p.Id == pizzaId).Single().PizzaName);
                    }
                    
                    model.Add(item);
                }

                return View(model);
            } else 
            {
                TempData["error"] = "Unauthorized access attempt!";
                return RedirectToAction("Login", "Home");
            }
            
        }
        public IActionResult OrdersHistory()
        {
            if (IsAdmin())
            {
                var ordersHistory = _context.OrdersHistory.OrderBy(oh => oh.CreatedAt).Select(oh => oh).ToList();
                return View(ordersHistory);
            } else
            {
                TempData["error"] = "Unauthorized access attempt!";
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public IActionResult ConfirmOrder(int orderId)
        {
            if (IsWorker())
            {
                OrderViewModel order = _context.Orders.Single(o => o.Id == orderId);
                order.OrderConfirmed = true;
                _context.Orders.Update(order);
                _context.SaveChanges();
                TempData["message"] = "Succesfully accepted order~";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        [HttpPost]
        public IActionResult RejectOrder(int orderId)
        {
            if (IsWorker())
            {
                OrderViewModel order = _context.Orders.Single(o => o.Id == orderId);
                OrderHistoryViewModel rejectedOrder = new OrderHistoryViewModel
                {
                    OrderId = order.Id,
                    OrderMakerName = order.OrderMakerName,
                    OrderPrice = order.OrderPrice,
                    CreatedAt = DateTime.Now,
                    OrderAccepted = false
                };

                _context.OrdersHistory.Add(rejectedOrder);
                _context.Orders.Remove(order);
                _context.SaveChanges();

                TempData["message"] = "Succesfully rejected order~";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult OrderReady(int orderId)
        {
            if (IsWorker())
            {
                OrderViewModel order = _context.Orders.Single(o => o.Id == orderId);
                OrderHistoryViewModel finishedOrder = new OrderHistoryViewModel
                {
                    OrderId = order.Id,
                    OrderMakerName = order.OrderMakerName,
                    OrderPrice = order.OrderPrice,
                    CreatedAt = DateTime.Now,
                    OrderAccepted = true
                };

                _context.OrdersHistory.Add(finishedOrder);
                _context.Orders.Remove(order);
                _context.SaveChanges();

                TempData["message"] = "Order is ready to be dished out~";
                return RedirectToAction(nameof(AcceptedOrders));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public ActionResult GetOrders()
        {
            if (IsWorker() || IsAdmin())    
            {
                List<ItemListHolderModel<OrderViewModel, string>> items = new List<ItemListHolderModel<OrderViewModel, string>>();
                var orders = _context.Orders.Where(o => o.OrderConfirmed == false).Select(o => o).ToList();
                foreach(var order in orders)
                {
                    ItemListHolderModel<OrderViewModel, string> item = new ItemListHolderModel<OrderViewModel, string>();
                    item.ItemA = order;
                    item.ItemsB = new List<string>();

                    //Get Pizzas ID
                    var orderList = _context.OrderItems.Where(oi => oi.OrderId == order.Id).Select(oi => oi).ToList();
                    foreach(var orderItem in orderList)
                    {
                        item.ItemsB.Add(_context.Pizzas.Single(p => p.Id == orderItem.PizzaId).PizzaName);
                    }
                    items.Add(item);
                }

                var jsonList = new List<object?>();
                foreach(var item in items)
                {
                    string pizzas = "";
                    for(int i =0; i < item.ItemsB.Count(); i++)
                    {
                        if(i + 1 != item.ItemsB.Count())
                        {
                            pizzas += item.ItemsB[i].ToString() + ", ";
                        } else
                        {
                            pizzas += item.ItemsB[i].ToString();
                        }
                    }
                    jsonList.Add(new { orderId = item.ItemA.Id, ordererName = item.ItemA.OrderMakerName, pizzas = pizzas, price = item.ItemA.OrderPrice });
                }
                return Json(jsonList);
            }
            else
            {
                return Json(new { data = "" });
            }
        }
        public ActionResult GetAcceptedOrders()
        {
            if (IsWorker() || IsAdmin())
            {
                List<ItemListHolderModel<OrderViewModel, string>> items = new List<ItemListHolderModel<OrderViewModel, string>>();
                var orders = _context.Orders.Where(o => o.OrderConfirmed == true).Select(o => o).ToList();
                foreach (var order in orders)
                {
                    ItemListHolderModel<OrderViewModel, string> item = new ItemListHolderModel<OrderViewModel, string>();
                    item.ItemA = order;
                    item.ItemsB = new List<string>();

                    //Get Pizzas ID
                    var orderList = _context.OrderItems.Where(oi => oi.OrderId == order.Id).Select(oi => oi).ToList();
                    foreach (var orderItem in orderList)
                    {
                        item.ItemsB.Add(_context.Pizzas.Single(p => p.Id == orderItem.PizzaId).PizzaName);
                    }
                    items.Add(item);
                }

                var jsonList = new List<object?>();
                foreach (var item in items)
                {
                    string pizzas = "";
                    for (int i = 0; i < item.ItemsB.Count(); i++)
                    {
                        if (i + 1 != item.ItemsB.Count())
                        {
                            pizzas += item.ItemsB[i].ToString() + ", ";
                        }
                        else
                        {
                            pizzas += item.ItemsB[i].ToString();
                        }
                    }
                    jsonList.Add(new { orderId = item.ItemA.Id, ordererName = item.ItemA.OrderMakerName, pizzas = pizzas, price = item.ItemA.OrderPrice });
                }
                return Json(jsonList);
            }
            else
            {
                return Json(new { data = "" });
            }
        }
    }
}
