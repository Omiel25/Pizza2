using Microsoft.AspNetCore.Mvc;
using Pizza2.Data;
using Pizza2.Models;
using Pizza2.Models.Orders;
using System.Text.Json;

namespace Pizza2.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index( )
        {
            if (IsWorker() || IsAdmin())
            {
                List<OrderDetailsModel> model = OrderDetailsModel.GetOrders( _context, false );

                return View( model );
            }
            else
            {
                TempData[ "error" ] = "Unauthorized access attempt!";
                return RedirectToAction( "Login", "Home" );
            }
        }

        public IActionResult AcceptedOrders( )
        {
            if (IsWorker())
            {
                List<OrderDetailsModel> model = OrderDetailsModel.GetOrders( _context, true );
                
                return View( model );
            }
            else
            {
                TempData[ "error" ] = "Unauthorized access attempt!";
                return RedirectToAction( "Login", "Home" );
            }

        }
        public IActionResult OrdersHistory( )
        {
            if (IsAdmin())
            {
                var ordersHistory = _context.OrdersHistory.OrderBy( oh => oh.CreatedAt ).Select( oh => oh ).ToList();
                return View( ordersHistory );
            }
            else
            {
                TempData[ "error" ] = "Unauthorized access attempt!";
                return RedirectToAction( "Login", "Home" );
            }
        }

        [HttpPost]
        public IActionResult ConfirmOrder(int orderId)
        {
            if (IsWorker())
            {
                OrderViewModel order = _context.Orders.Single( o => o.Id == orderId );
                order.OrderConfirmed = true;
                _context.Orders.Update( order );
                _context.SaveChanges();
                TempData[ "message" ] = "Succesfully accepted order~";
                return RedirectToAction( nameof( Index ) );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        [HttpPost]
        public IActionResult RejectOrder(int orderId)
        {
            if (IsWorker())
            {
                OrderViewModel order = _context.Orders.Single( o => o.Id == orderId );
                OrderHistoryViewModel rejectedOrder = new OrderHistoryViewModel
                {
                    OrderId = order.Id,
                    OrderMakerName = order.OrderMakerName,
                    OrderPrice = order.OrderPrice,
                    CreatedAt = DateTime.Now,
                    OrderAccepted = false
                };

                _context.OrdersHistory.Add( rejectedOrder );
                _context.Orders.Remove( order );
                _context.SaveChanges();

                TempData[ "message" ] = "Succesfully rejected order~";
                return RedirectToAction( nameof( Index ) );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }
        }

        [HttpPost]
        public ActionResult OrderReady(int orderId)
        {
            if (IsWorker())
            {
                OrderViewModel order = _context.Orders.Single( o => o.Id == orderId );
                OrderHistoryViewModel finishedOrder = new OrderHistoryViewModel
                {
                    OrderId = order.Id,
                    OrderMakerName = order.OrderMakerName,
                    OrderPrice = order.OrderPrice,
                    CreatedAt = DateTime.Now,
                    OrderAccepted = true
                };

                _context.OrdersHistory.Add( finishedOrder );
                _context.Orders.Remove( order );
                _context.SaveChanges();

                TempData[ "message" ] = "Order is ready to be dished out~";
                return RedirectToAction( nameof( AcceptedOrders ) );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }
        public ActionResult GetOrders( )
        {
            if (IsWorker() || IsAdmin())
            {
                List<OrderDetailsModel> model = OrderDetailsModel.GetOrders( _context, false );

                var jsonList = new List<object?>();
                foreach (OrderDetailsModel acceptedOrder in model)
                {
                    string text = "";
                    float pizzaPrice = 0f;
                    foreach (PizzaSubModel pizzaContainer in acceptedOrder.OrderedPizzas)
                    {
                        text += pizzaContainer.CreatePizzaText();
                        pizzaPrice += pizzaContainer.Pizza.PizzaPrice ?? 0;
                    }

                    jsonList.Add( new
                    {
                        name = acceptedOrder.OrderData.OrderMakerName,
                        id = acceptedOrder.OrderData.Id,
                        pizzas = text,
                        price = pizzaPrice
                    } );
                }

                var options = new JsonSerializerOptions();
                options.PropertyNamingPolicy = null;

                return Json( jsonList, options );
            }
            else
            {
                return Json( new { data = "" } );
            }
        }

        [HttpPost]
        public JsonResult GetAcceptedOrders( )
        {
            if (IsWorker() || IsAdmin())
            {
                List<OrderDetailsModel> model = OrderDetailsModel.GetOrders( _context, true );

                var jsonList = new List<object?>();
                foreach (OrderDetailsModel acceptedOrder in model)
                {
                    string text = "";
                    foreach (PizzaSubModel pizzaContainer in acceptedOrder.OrderedPizzas)
                    {
                        text += pizzaContainer.CreatePizzaText();
                    }

                    jsonList.Add( new
                    {
                        ordererName = acceptedOrder.OrderData.OrderMakerName,
                        orderId = acceptedOrder.OrderData.Id,
                        pizzaText = text,
                    } );
                }

                var options = new JsonSerializerOptions();
                options.PropertyNamingPolicy = null;

                return Json( jsonList, options );
            }
            else
            {
                return Json( new { data = "" } );
            }
        }
    }
}
