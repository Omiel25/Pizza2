using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizza2.Data;
using Pizza2.Models;

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
                AddMultipleModel<PizzaViewModel, int> model = new AddMultipleModel<PizzaViewModel, int>();

                //Get uniqe values for ingridientListId
                var result = _context.PizzaIngridients.Select(p => p.PizzaIngridientListId).Distinct().ToList();

                model.itemTwoList = result;

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult CreatePizza(AddMultipleModel<PizzaViewModel, int> model)
        {
            if (IsAdmin())
            {
                PizzaViewModel pizza = model.itemOne;
                float price = 0;
                if (model.itemTwoList.Count() == 2)
                {
                    if (model.itemTwoList[0] > 0 && model.itemTwoList[0] > 0)
                    {
                        price = float.Parse($"{model.itemTwoList[0]},{model.itemTwoList[1]}");
                    }
                    else if (model.itemTwoList[0] > 0 && model.itemTwoList[1] == 0)
                    {
                        price = float.Parse($"{model.itemTwoList[0]},00");
                    }
                    else if (model.itemTwoList[0] == 0 && model.itemTwoList[1] > 0)
                    {
                        price = float.Parse($"0,{model.itemTwoList[1]}");
                    }

                    pizza.PizzaPrice = price;
                }

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

                TempData["message"] = "Succesfully added new Pizza!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
    
       
    }
}
