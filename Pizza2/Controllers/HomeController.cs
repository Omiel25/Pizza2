﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LoginUser(UserViewModel user)
        {
            List<UserViewModel> matchUser;
            if(!String.IsNullOrWhiteSpace(user.UserName) && !String.IsNullOrWhiteSpace(user.Password))
            {
                string a =_context.Database.GetConnectionString();
                bool b = _context.Database.CanConnect();
                matchUser = _context.User.Where(u => u.Password == user.Password && u.UserName == user.UserName).Select(u => u).ToList();

                if(matchUser.Count() < 1)
                {
                    //If there is less than one user
                    TempData["error"] = "No User found!";
                    return RedirectToAction(nameof(Login));
                } else if (matchUser.Count() > 1) {

                    //If there is more than one user
                    TempData["error"] = "Error: Too many Users found!";

                    return RedirectToAction(nameof(Login));
                } else {

                    //If there is only one user
                    SetSessionPrivilages(matchUser[0].UserName, matchUser[0].Privilages.ToString());

                    return RedirectToAction("Index", "Home");
                }
            } else {
                TempData["message"] = "Fields can't be left empty!";
                return RedirectToAction(nameof(Login));
            }

            

        }

        public IActionResult Index()
        {
            //Use tempdata to check
            if (IsUser())
            {
                ViewData["TableName"] = GetSessionUsername();
                return RedirectToAction(nameof(AvalibleMenu));
            }
            else if (IsWorker() && !IsAdmin())
            {
                //worker station panel (Menu + Orders)
                return RedirectToAction("Index", "Orders");
            }
            else if (IsAdmin())
            {
                //Admin Panel (Everything besides Menu)
                return View();
            } else {
                TempData["error"] = "Unauthorized access!";
                return View("Login");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AvalibleMenu()
        {
            //Need to change it- get both pizzas AND Ingridients names (Maybe make new model, idk)
            if(IsUser() == false && IsWorker() == false)
            {
                TempData["error"] = "Unauthorized access!";
                return View("Login");

            } else if (IsWorker() || IsUser()) {

                //Initiate List Holder
                List<ItemListHolderModel<PizzaViewModel, string>> model = new List<ItemListHolderModel<PizzaViewModel, string>>();

                //Get pizzas (ItemA)
                //var pizzas = _context.Pizzas.OrderBy(p => p.PizzaPrice).Select(p => p).ToList();
                var pizzas = from menu in _context.Menu
                              join p in _context.Pizzas
                              on menu.PizzaId equals p.Id
                              where menu.IsActive
                              select p;

                foreach (var item in pizzas)
                {
                    ItemListHolderModel<PizzaViewModel, string> pizza = new ItemListHolderModel<PizzaViewModel, string>();
                    pizza.ItemsB = new List<string>();
                    pizza.ItemA = item;

                    //Get list of ingridnets names for target pizza and fill our model with it
                    var query = from list in _context.PizzaIngridients
                                join i in _context.Ingridients on list.IngridientId equals i.Id
                                orderby i.DisplayPriority ascending
                                select new
                                {
                                    ingridientListId = list.PizzaIngridientListId,
                                    ingridientName = i.IngridientName
                                };

                    foreach (var ingridient in query)
                    {
                        if (ingridient.ingridientListId == pizza.ItemA.IngridientsListId)
                        {
                            pizza.ItemsB.Add(ingridient.ingridientName);
                        }
                    }

                    model.Add(pizza);
                }


                TempData["privilages"] = GetSessionPrivilages();
                return View(model);
            } else {
                TempData["error"] = "Something went wrong!";
                return View("Login");
            }
            
        }

        public IActionResult Order(List<ItemListHolderModel<PizzaViewModel, string>> model)
        {
            if (IsUser() || IsWorker())
            {
                if(model.Count() != 0)
                {
                    //Get user name with session 
                    //Send chosen pizzas id's
                    List<PizzaViewModel> selectedPizzas = new List<PizzaViewModel>();

                    foreach (var item in model)
                    {
                        selectedPizzas.Add(_context.Pizzas.Single(p => p.Id == item.Id));
                    }

                    ViewData["User"] = GetSessionUsername();
                    TempData["privilages"] = GetSessionPrivilages();

                    return View(selectedPizzas);
                } else
                {
                    TempData["error"] = "Select your order first"!;
                    return RedirectToAction("Index");
                }
                
            }
            else
            {
                TempData["error"] = "Unauthorized access!";
                return View("Login");
            }
        }
        
        public IActionResult MakeOrder(List<PizzaViewModel> model)
        {
            if (IsUser() || IsWorker())
            {
                if(model.Count() != 0)
                {
                    //Get selected pizzas and fill needed variables
                    List<PizzaViewModel> selectedPizzas = new List<PizzaViewModel>();
                    float? totalPrice = 0;
                    string orderMakerName = GetSessionUsername();
                    foreach (var item in model)
                    {
                        selectedPizzas.Add(_context.Pizzas.Single(p => p.Id == item.Id));
                    }

                    foreach (var p in selectedPizzas)
                    {
                        totalPrice += p.PizzaPrice;
                    }

                    //Create Order, add it to database and then create and fill OrderList
                    OrderViewModel order = new OrderViewModel
                    {
                        OrderConfirmed = false,
                        OrderPrice = totalPrice,
                        OrderMakerName = orderMakerName
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    List<OrderItemsViewModel> orderList = new List<OrderItemsViewModel>();
                    foreach(var item in selectedPizzas)
                    {
                        OrderItemsViewModel orderItem  = new OrderItemsViewModel
                        {
                            PizzaId = item.Id,
                            OrderId = order.Id
                        };

                        orderList.Add(orderItem);
                    }

                    _context.OrderItems.AddRange(orderList);
                    _context.SaveChanges();

                    //If order maker is USer redirect them to thank you page nad go back to index after few seconds
                    if (IsUser())
                    {
                        return RedirectToAction(nameof(SeeOrder), order);
                    } else {
                        return RedirectToAction(nameof(Index), "Orders");
                    }
                    
                    //return View();
                } else
                {
                    TempData["error"] = "Select your order first"!;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["error"] = "Unauthorized access!";
                return View("Login");
            }
        }

        public IActionResult SeeOrder(OrderViewModel order)
        {
            return View(order);
        }
        public IActionResult Logout()
        {
            if (IsUser() || IsWorker() || IsAdmin())
            {
                UnsetSessionPrivilages();
                return View("Login");
                
            } else
            {
                TempData["error"] = "Unauthorized access!";
                return View("Login");
            }
        }

        public JsonResult GetData(string data)
        {
            int id = Convert.ToInt32(data);
            var confirmation = false;
            try
            {
                confirmation = _context.Orders.Single(p => p.Id == id).OrderConfirmed;
            } catch(Exception e)
            {
                confirmation = false;
            }
           

            return Json(new { isConfirmed = confirmation});
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}