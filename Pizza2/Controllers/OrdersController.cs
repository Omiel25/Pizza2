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
                List<AcceptedOrderModel> model = new List<AcceptedOrderModel>();

                //Needed or custom pizzas
                List<IngridientViewModel> ingridients = _context.Ingridients.ToList();

                List<OrderViewModel> orders = _context.Orders.Where( p => p.OrderConfirmed == false ).ToList();

                foreach (OrderViewModel order in orders)
                {
                    AcceptedOrderModel newOrder = new AcceptedOrderModel();
                    newOrder.OrderedPizzas = new List<PizzaSubModel>();
                    newOrder.OrderData = order;

                    List<int> pizzasId = _context.OrderItems.Where( i => i.OrderId == order.Id ).Select( i => i.PizzaId ).ToList();
                    List<PizzaViewModel> orderPizzas = _context.Pizzas.Where( p => pizzasId.Contains( p.Id ) ).ToList();

                    foreach (PizzaViewModel orderedPizza in orderPizzas)
                    {
                        PizzaSubModel pizzaModel = new PizzaSubModel();
                        pizzaModel.Pizza = orderedPizza;

                        if (orderedPizza.IsCustomPizza == true)
                        {
                            List<int> customIngridientsId = _context.CustomPizzaIngridients.
                                Where( i => i.PizzaID == orderedPizza.Id ).
                                Select( i => i.IngridientID ).
                                ToList();

                            pizzaModel.PizzaIngridients = _context.Ingridients.
                                Where( i => customIngridientsId.Contains( i.Id ) ).
                                OrderBy( i => i.DisplayPriority ).
                                ToList();
                        }

                        newOrder.OrderedPizzas.Add( pizzaModel );
                    }

                    model.Add( newOrder );
                }
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
                List<AcceptedOrderModel> model = new List<AcceptedOrderModel>();

                //Needed or custom pizzas
                List<IngridientViewModel> ingridients = _context.Ingridients.ToList();

                List<OrderViewModel> acceptedOrders = _context.Orders.Where( p => p.OrderConfirmed == true ).ToList();

                foreach (OrderViewModel acceptedOrder in acceptedOrders)
                {
                    AcceptedOrderModel newOrder = new AcceptedOrderModel();
                    newOrder.OrderedPizzas = new List<PizzaSubModel>();
                    newOrder.OrderData = acceptedOrder;

                    List<int> pizzasId = _context.OrderItems.Where( i => i.OrderId == acceptedOrder.Id ).Select( i => i.PizzaId ).ToList();
                    List<PizzaViewModel> orderPizzas = _context.Pizzas.Where( p => pizzasId.Contains( p.Id ) ).ToList();

                    foreach (PizzaViewModel orderedPizza in orderPizzas)
                    {
                        PizzaSubModel pizzaModel = new PizzaSubModel();
                        pizzaModel.Pizza = orderedPizza;

                        if (orderedPizza.IsCustomPizza == true)
                        {
                            List<int> customIngridientsId = _context.CustomPizzaIngridients.
                                Where( i => i.PizzaID == orderedPizza.Id ).
                                Select( i => i.IngridientID ).
                                ToList();

                            pizzaModel.PizzaIngridients = _context.Ingridients.
                                Where( i => customIngridientsId.Contains( i.Id ) ).
                                OrderBy( i => i.DisplayPriority ).
                                ToList();
                        }

                        newOrder.OrderedPizzas.Add( pizzaModel );
                    }

                    model.Add( newOrder );
                }
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
                List<AcceptedOrderModel> model = new List<AcceptedOrderModel>();

                //Needed or custom pizzas
                List<IngridientViewModel> ingridients = _context.Ingridients.ToList();

                List<OrderViewModel> acceptedOrders = _context.Orders.Where( p => p.OrderConfirmed == false ).ToList();

                foreach (OrderViewModel acceptedOrder in acceptedOrders)
                {
                    AcceptedOrderModel newOrder = new AcceptedOrderModel();
                    newOrder.OrderedPizzas = new List<PizzaSubModel>();
                    newOrder.OrderData = acceptedOrder;

                    List<int> pizzasId = _context.OrderItems.Where( i => i.OrderId == acceptedOrder.Id ).Select( i => i.PizzaId ).ToList();
                    List<PizzaViewModel> orderPizzas = _context.Pizzas.Where( p => pizzasId.Contains( p.Id ) ).ToList();

                    //Bind ingridients to target pizzas
                    float orderPrice = 0f;
                    foreach (PizzaViewModel orderedPizza in orderPizzas)
                    {
                        PizzaSubModel pizzaModel = new PizzaSubModel();
                        pizzaModel.Pizza = orderedPizza;

                        if (orderedPizza.IsCustomPizza == true)
                        {
                            List<int> customIngridientsId = _context.CustomPizzaIngridients.
                                Where( i => i.PizzaID == orderedPizza.Id ).
                                Select( i => i.IngridientID ).
                                ToList();

                            pizzaModel.PizzaIngridients = _context.Ingridients.
                                Where( i => customIngridientsId.Contains( i.Id ) ).
                                OrderBy( i => i.DisplayPriority ).
                                ToList();
                        }
                        //Calculate Automatic pizza price
                        if (pizzaModel.Pizza.PizzaPrice == null || pizzaModel.Pizza.PizzaPrice <= 0f)
                        {
                            List<int> ingridientsId = _context.PizzaIngridients.
                                Where( i => i.PizzaIngridientListId == pizzaModel.Pizza.IngridientsListId ).
                                Select( i => i.IngridientId ).
                                ToList();

                            pizzaModel.Pizza.CalculateCustomPrice( ingridientsId, ingridients );
                        }

                        orderPrice += pizzaModel.Pizza.PizzaPrice ?? 0f;
                        newOrder.OrderedPizzas.Add( pizzaModel );
                    }

                    newOrder.OrderData.OrderPrice = orderPrice;
                    model.Add( newOrder );
                }

                List<AcceptedOrdersAjaxModel> ajaxObjects = new List<AcceptedOrdersAjaxModel>();
                foreach (AcceptedOrderModel acceptedOrder in model)
                {
                    string text = "";
                    float pizzaPrice = 0f;
                    foreach (PizzaSubModel pizzaContainer in acceptedOrder.OrderedPizzas)
                    {
                        if (pizzaContainer.Pizza.IsCustomPizza == false)
                        {
                            text += $"<p>{pizzaContainer.Pizza.PizzaName}</p>";
                        }
                        else
                        {
                            int ingridinentCount = pizzaContainer.PizzaIngridients.Count();
                            text += $"<p class=\"fw-bold\">{pizzaContainer.Pizza.PizzaName} (<span>";
                            for (int i = 0; i < ingridinentCount; i++)
                            {
                                if (i + 1 == ingridinentCount)
                                    text += pizzaContainer.PizzaIngridients[ i ].IngridientName;
                                else
                                    text += $"{pizzaContainer.PizzaIngridients[ i ].IngridientName}, ";
                            }
                            text += $"</span>) </p>";
                        }

                        pizzaPrice += pizzaContainer.Pizza.PizzaPrice ?? 0;
                    }

                    AcceptedOrdersAjaxModel ajaxObject = new AcceptedOrdersAjaxModel()
                    {
                        orderId = acceptedOrder.OrderData.Id,
                        ordererName = acceptedOrder.OrderData.OrderMakerName,
                        pizzasText = text,
                        pizzaPrice = pizzaPrice
                    };

                    ajaxObjects.Add( ajaxObject );
                }

                var options = new JsonSerializerOptions();
                options.PropertyNamingPolicy = null;

                var jsonList = new List<object?>();
                foreach (var item in ajaxObjects)
                {
                    jsonList.Add( new { 
                        id = item.orderId, 
                        name = item.ordererName,
                        pizzas = item.pizzasText,
                        price = item.pizzaPrice
                    } );
                }
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
                List<AcceptedOrderModel> model = new List<AcceptedOrderModel>();

                //Needed or custom pizzas
                List<IngridientViewModel> ingridients = _context.Ingridients.ToList();

                List<OrderViewModel> acceptedOrders = _context.Orders.Where( p => p.OrderConfirmed == true ).ToList();

                foreach (OrderViewModel acceptedOrder in acceptedOrders)
                {
                    AcceptedOrderModel newOrder = new AcceptedOrderModel();
                    newOrder.OrderedPizzas = new List<PizzaSubModel>();
                    newOrder.OrderData = acceptedOrder;

                    List<int> pizzasId = _context.OrderItems.Where( i => i.OrderId == acceptedOrder.Id ).Select( i => i.PizzaId ).ToList();
                    List<PizzaViewModel> orderPizzas = _context.Pizzas.Where( p => pizzasId.Contains( p.Id ) ).ToList();

                    foreach (PizzaViewModel orderedPizza in orderPizzas)
                    {
                        PizzaSubModel pizzaModel = new PizzaSubModel();
                        pizzaModel.Pizza = orderedPizza;

                        if (orderedPizza.IsCustomPizza == true)
                        {
                            List<int> customIngridientsId = _context.CustomPizzaIngridients.
                                Where( i => i.PizzaID == orderedPizza.Id ).
                                Select( i => i.IngridientID ).
                                ToList();

                            pizzaModel.PizzaIngridients = _context.Ingridients.
                                Where( i => customIngridientsId.Contains( i.Id ) ).
                                OrderBy( i => i.DisplayPriority ).
                                ToList();
                        }

                        newOrder.OrderedPizzas.Add( pizzaModel );
                    }

                    model.Add( newOrder );
                }

                List<AcceptedOrdersAjaxModel> ajaxObjects = new List<AcceptedOrdersAjaxModel>();
                foreach (AcceptedOrderModel acceptedOrder in model)
                {
                    string text = "";
                    foreach (PizzaSubModel pizzaContainer in acceptedOrder.OrderedPizzas)
                    {
                        if (pizzaContainer.Pizza.IsCustomPizza == false)
                        {
                            text += $"<p>{pizzaContainer.Pizza.PizzaName}</p>";
                        }
                        else
                        {
                            int ingridinentCount = pizzaContainer.PizzaIngridients.Count();
                            text += $"<p class=\"fw-bold\">{pizzaContainer.Pizza.PizzaName} (<span>";
                            for (int i = 0; i < ingridinentCount; i++)
                            {
                                if (i + 1 == ingridinentCount)
                                    text += pizzaContainer.PizzaIngridients[ i ].IngridientName;
                                else
                                    text += $"{pizzaContainer.PizzaIngridients[ i ].IngridientName},";
                            }
                            text += $"</span>) </p>";
                        }
                    }
                    AcceptedOrdersAjaxModel ajaxObject = new AcceptedOrdersAjaxModel()
                    {
                        orderId = acceptedOrder.OrderData.Id,
                        ordererName = acceptedOrder.OrderData.OrderMakerName,
                        pizzasText = text
                    };

                    ajaxObjects.Add( ajaxObject );
                }

                var options = new JsonSerializerOptions();
                options.PropertyNamingPolicy = null;

                var jsonList = new List<object?>();
                foreach (var item in ajaxObjects)
                {
                    jsonList.Add( new { ordererName = item.ordererName, orderId = item.orderId, pizzaText = item.pizzasText } );
                }

                return Json( jsonList, options );
            }
            else
            {
                return Json( new { data = "" });
            }
        }
    }
}
