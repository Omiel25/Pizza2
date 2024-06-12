using Microsoft.AspNetCore.Mvc;
using Pizza2.Data;
using Pizza2.Models;
using System.Diagnostics;

namespace Pizza2.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Login( )
        {
            return View();
        }

        public IActionResult LoginUser(UserViewModel user)
        {
            List<UserViewModel> matchUser;
            if (!String.IsNullOrWhiteSpace( user.UserName ) && !String.IsNullOrWhiteSpace( user.Password ))
            {
                matchUser = _context.User.Where( u => u.Password == user.Password && u.UserName == user.UserName ).Select( u => u ).ToList();

                if (matchUser.Count() < 1)
                {
                    //If there is less than one user
                    TempData[ "error" ] = "No User found!";
                    return RedirectToAction( nameof( Login ) );
                }
                else if (matchUser.Count() > 1)
                {

                    //If there is more than one user
                    TempData[ "error" ] = "Error: Too many Users found!";

                    return RedirectToAction( nameof( Login ) );
                }
                else
                {

                    //If there is only one user
                    SetSessionPrivilages( matchUser[ 0 ].UserName, matchUser[ 0 ].Privilages.ToString() );

                    return RedirectToAction( "Index", "Home" );
                }
            }
            else
            {
                TempData[ "message" ] = "Fields can't be left empty!";
                return RedirectToAction( nameof( Login ) );
            }



        }

        public IActionResult Index( )
        {
            //Use tempdata to check
            if (IsUser())
            {
                ViewData[ "TableName" ] = GetSessionUsername();
                return RedirectToAction( nameof( AvalibleMenu ) );
            }
            else if (IsWorker() && !IsAdmin())
            {
                //worker station panel (Menu + Orders)
                return RedirectToAction( "Index", "Orders" );
            }
            else if (IsAdmin())
            {
                //Admin Panel (Everything besides Menu)
                return View();
            }
            else
            {
                TempData[ "error" ] = "Unauthorized access!";
                return View( "Login" );
            }
        }

        public IActionResult Privacy( )
        {
            return View();
        }

        public IActionResult AvalibleMenu(List<int>? customPizzaIds)
        {
            if (IsWorker() || IsUser())
            {
                AvalibleMenuModel model = new AvalibleMenuModel();
                model.Pizzas = new List<PizzaSubModel>();
                model.ShopCartPizzasIds = new List<string>();

                //Get pizzas from active menu (ItemA)
                var pizzas = from menu in _context.Menu
                             join p in _context.Pizzas
                             on menu.PizzaId equals p.Id
                             where menu.IsActive
                             select p;

                List<IngridientViewModel> ingridientList = _context.Ingridients.OrderBy( p => p.DisplayPriority ).ToList();
                model.Ingridients = ingridientList;

                foreach (var pizza in pizzas)
                {
                    //Item containing <Pizza, ingridients names>
                    PizzaSubModel pizzaHolder = new PizzaSubModel();
                    pizzaHolder.PizzaIngridients = new List<IngridientViewModel>();
                    pizzaHolder.Pizza = pizza;

                    //Get list of ingridnets names for target pizza and fill our model with it
                    var query = from list in _context.PizzaIngridients
                                join i in _context.Ingridients on list.IngridientId equals i.Id
                                orderby i.DisplayPriority ascending
                                select new
                                {
                                    ingridientId = list.IngridientId,
                                    ingridientListId = list.PizzaIngridientListId,
                                    ingridientName = i.IngridientName,
                                    ingridientPrice = i.IngridientPrice
                                };

                    List<int> pizzaIngridientIds = _context.PizzaIngridients
                        .Where( pi => pi.PizzaIngridientListId == pizza.IngridientsListId )
                        .Select( pi => pi.IngridientId )
                        .ToList();

                    pizzaHolder.PizzaIngridients = ingridientList.Where( i => pizzaIngridientIds.Contains( i.Id ) ).ToList();


                    if (pizzaHolder.Pizza.PizzaPrice == null)
                    {
                        List<int> ingridientsID = query.Where( i => i.ingridientListId == pizza.IngridientsListId )
                            .Select( p => p.ingridientId ).ToList();

                        if (ingridientsID.Count == 0)
                        {
                            SetErrorMessage( "Couldn't find needed ingridients..." );
                            return View( "Login" );
                        }
                        pizzaHolder.Pizza.CalculateCustomPrice(ingridientsID, ingridientList );
                    }

                    model.Pizzas.Add( pizzaHolder );
                }


                TempData[ "privilages" ] = GetSessionPrivilages();
                return View( model );
            }
            else
            {
                TempData[ "error" ] = "Unauthorized access!";
                return View( "Login" );
            }

        }

        public IActionResult Order(IFormCollection collection)
        {
            if (IsUser() || IsWorker())
            {
                if (collection.Count() != 0)
                {
                    //Get user name with session 
                    //Send chosen pizzas id's
                    List<PizzaViewModel> selectedPizzas = new List<PizzaViewModel>();

                    foreach (var item in collection)
                    {
                        if (!item.Key.Contains( "Pizza", StringComparison.OrdinalIgnoreCase ))
                            continue;

                        int PizzaId = int.Parse(item.Value);
                        PizzaViewModel pizza = _context.Pizzas.Single( p => p.Id == PizzaId );

                        //Add price to custom pizza or pizza with automatic price calculation
                        List<int> pizzaIngridinetsId;
                        if(pizza.IsCustomPizza || pizza.PizzaPrice == null)
                        {
                            if (pizza.IsCustomPizza == true)
                            {
                                pizzaIngridinetsId = _context.CustomPizzaIngridients.
                                    Where( c => c.PizzaID == pizza.Id ).
                                    Select( c => c.IngridientID ).
                                    ToList();
                            }
                            else
                            {
                                pizzaIngridinetsId = _context.PizzaIngridients.
                                    Where( c => c.PizzaIngridientListId == pizza.IngridientsListId ).
                                    Select( c => c.IngridientId ).
                                    ToList();
                            }

                            List<IngridientViewModel> ingridients = _context.Ingridients.ToList();
                            pizza.CalculateCustomPrice( pizzaIngridinetsId, ingridients );
                        }

                        selectedPizzas.Add( pizza );
                    }

                    ViewData[ "User" ] = GetSessionUsername();
                    TempData[ "privilages" ] = GetSessionPrivilages();

                    return View( selectedPizzas );
                }
                else
                {
                    TempData[ "error" ] = "Select your order first"!;
                    return RedirectToAction( "Index" );
                }

            }
            else
            {
                TempData[ "error" ] = "Unauthorized access!";
                return View( "Login" );
            }
        }

        public IActionResult MakeOrder(List<PizzaViewModel> model)
        {
            if (IsUser() || IsWorker())
            {
                if (model.Count() != 0)
                {
                    //Get ingridients for custom pizzas 
                    List<IngridientViewModel> ingridients = _context.Ingridients.ToList();
                    //Get selected pizzas and fill needed variables
                    List<PizzaViewModel> selectedPizzas = new List<PizzaViewModel>();
                    float? totalPrice = 0;
                    string orderMakerName = GetSessionUsername();
                    foreach (var item in model)
                    {
                        selectedPizzas.Add( _context.Pizzas.Single( p => p.Id == item.Id ) );
                    }

                    foreach (var p in selectedPizzas)
                    {
                        List<int> customIngridientsId;
                        if(p.PizzaPrice == null)
                        {
                            if(p.IsCustomPizza == true)
                            {
                                customIngridientsId = _context.CustomPizzaIngridients.
                                Where( c => c.PizzaID == p.Id ).
                                Select( c => c.IngridientID ).
                                ToList();
                            } else
                            {
                                customIngridientsId = _context.PizzaIngridients.
                                Where( i => i.PizzaIngridientListId == p.IngridientsListId ).
                                Select( c => c.IngridientId ).
                                ToList();

                            }

                            p.CalculateCustomPrice( customIngridientsId, ingridients );
                        }

                        totalPrice += p.PizzaPrice;
                    }

                    //Create Order, add it to database and then create and fill OrderList
                    OrderViewModel order = new OrderViewModel
                    {
                        OrderConfirmed = false,
                        OrderPrice = totalPrice,
                        OrderMakerName = orderMakerName
                    };

                    _context.Orders.Add( order );
                    _context.SaveChanges();

                    List<OrderItemsViewModel> orderList = new List<OrderItemsViewModel>();
                    foreach (var item in selectedPizzas)
                    {
                        OrderItemsViewModel orderItem = new OrderItemsViewModel
                        {
                            PizzaId = item.Id,
                            OrderId = order.Id
                        };

                        orderList.Add( orderItem );
                    }

                    _context.OrderItems.AddRange( orderList );
                    _context.SaveChanges();

                    //If order maker is User redirect them to thank you page nad go back to index after few seconds
                    if (IsUser())
                    {
                        return RedirectToAction( nameof( SeeOrder ), order );
                    }
                    else
                    {
                        return RedirectToAction( nameof( Index ), "Orders" );
                    }

                    //return View();
                }
                else
                {
                    TempData[ "error" ] = "Select your order first"!;
                    return RedirectToAction( "Index" );
                }
            }
            else
            {
                TempData[ "error" ] = "Unauthorized access!";
                return View( "Login" );
            }
        }

        public IActionResult SeeOrder(OrderViewModel order)
        {
            return View( order );
        }
        public IActionResult Logout( )
        {
            if (IsUser() || IsWorker() || IsAdmin())
            {
                UnsetSessionPrivilages();
                return View( "Login" );

            }
            else
            {
                TempData[ "error" ] = "Unauthorized access!";
                return View( "Login" );
            }
        }

        public JsonResult GetData(string data)
        {
            int id = Convert.ToInt32( data );
            var confirmation = false;
            try
            {
                confirmation = _context.Orders.Single( p => p.Id == id ).OrderConfirmed;
            }
            catch (Exception e)
            {
                confirmation = false;
            }


            return Json( new { isConfirmed = confirmation } );
        }

        public JsonResult CreateCustomPizza(string[] ingridients, string name)
        {
            PizzaViewModel customPizza = new PizzaViewModel();
            customPizza.IsCustomPizza = true;
            customPizza.PizzaName = String.IsNullOrEmpty( name ) ? "Custom Pizza" : name ;

            _context.Pizzas.Add( customPizza );
            _context.SaveChanges();

            int pizzaID = customPizza.Id;

            List<IngridientViewModel> pizzaIngridients = _context.Ingridients.
            Where( i =>
                ingridients.Contains( i.IngridientName ) ||
                i.IngridientName == "Pizza Pie" )
            .ToList();

            foreach (IngridientViewModel ingridient in pizzaIngridients)
            {
                CustomPizzaIngridientsViewModel customPizzaIngridient = new CustomPizzaIngridientsViewModel()
                {
                    PizzaID = pizzaID,
                    IngridientID = ingridient.Id
                };

                _context.CustomPizzaIngridients.Add( customPizzaIngridient );
            }

            _context.SaveChanges();

            return Json( new { addedPizzaID = pizzaID } );
        }

        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error( )
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }

    }
}