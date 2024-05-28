using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizza2.Data;
using Pizza2.Models;
using System.Collections.Generic;

namespace Pizza2.Controllers
{
    public class PizzaController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public PizzaController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            if (IsAdmin())
            {
                return View();
            } else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult Create()
        {
            if (IsAdmin())
            {
                AddMultipleModel<PizzaViewModel, Dictionary<int,string>> model = new AddMultipleModel<PizzaViewModel, Dictionary<int, string>>();

                //Get uniqe values for ingridientListId
                var result = _context.PizzaIngridients
                    .Select( pi => new { pi.PizzaIngridientListId, pi.IngridientListName } )
                    .Distinct()
                    .ToList();

                model.itemTwo = new Dictionary<int, string>();

                foreach(var pi in result)
                {
                    model.itemTwo.Add( pi.PizzaIngridientListId, pi.IngridientListName );
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult CreatePizza(AddMultipleModel<PizzaViewModel, string> model)
        {
            if (IsAdmin())
            {
                PizzaViewModel pizza = model.itemOne;
                if (float.TryParse( model.itemTwo?.Replace( ".", "," ), out float itemPrice ))
                {
                    pizza.PizzaPrice = itemPrice;
                }
                else
                    pizza.PizzaPrice = null;

                pizza.IsCustomPizza = false;

                _context.Pizzas.Add(pizza);
                _context.SaveChanges();

                TempData["message"] = "Succesfully added new Pizza!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult Details()
        {
            if (IsAdmin())
            {
                //Initiate List Holder
                List<ItemListHolderModel<PizzaViewModel, string>> model = new List<ItemListHolderModel<PizzaViewModel, string>>();

                List<PizzaViewModel> pizzas = _context.Pizzas.Where(p => p.IsCustomPizza == false).ToList();

                foreach(PizzaViewModel pizza in pizzas)
                {
                    ItemListHolderModel<PizzaViewModel, string> pizzaData = new ItemListHolderModel<PizzaViewModel, string>();
                    pizzaData.ItemsB = new List<string>();
                    pizzaData.ItemA = pizza;

                    float automaticPrice = 0f;

                    //Get list of ingridnets names for target pizza and fill our model with it
                    var query = from list in _context.PizzaIngridients
                                join i in _context.Ingridients on list.IngridientId equals i.Id
                                orderby i.DisplayPriority ascending
                                select new
                                {
                                    ingridientListId = list.PizzaIngridientListId,
                                    ingridientListName = list.IngridientListName,
                                    ingridientName = i.IngridientName,
                                    ingridientPrice = i.IngridientPrice
                                };
                    
                    foreach (var ingridient in query)
                    {
                        if (ingridient.ingridientListId == pizza.IngridientsListId)
                        {
                            pizzaData.ItemsB.Add( ingridient.ingridientName );

                            if (pizza.PizzaPrice == null)
                                automaticPrice += ingridient.ingridientPrice;
                        }
                    }

                    foreach(var ingridient in query)
                    {
                        if (ingridient.ingridientListId == pizza.IngridientsListId)
                        {
                            pizzaData.ItemB = $"{ingridient.ingridientListId} - {ingridient.ingridientListName}";
                            break;
                        }
                    }

                    if (automaticPrice != 0f)
                        pizzaData.customData = $"{automaticPrice}";
                    model.Add( pizzaData );
                }

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult Delete()
        {
            if (IsAdmin())
            {
                //Initiate List Holder
                List<ItemListHolderModel<PizzaViewModel, string>> model = new List<ItemListHolderModel<PizzaViewModel, string>>();

                //Get pizzas (ItemA)
                var pizzas = _context.Pizzas.Select(d => d).ToList();

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

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
    
        public IActionResult DeletePizza(List<ItemListHolderModel<PizzaViewModel, string>> model)
        {
            if (IsAdmin())
            {
                var pizza = new PizzaViewModel() { Id = model[0].Id };

                if (pizza.Id >= 0)
                {
                    _context.Pizzas.Remove(pizza);
                    _context.SaveChanges();
                }

                SetMessage( "Successfully deleted pizza" );
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
    
       
    }
}
