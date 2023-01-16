using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pizza2.Data;
using Pizza2.Models;
using System.Linq;

namespace Pizza2.Controllers
{
    public class IngridientsListController : BaseController
    {

        private readonly ApplicationDbContext _context;

        public IngridientsListController(ApplicationDbContext context)
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
                AddMultipleModel<IngridientViewModel, ItemHolderModel<IngridientViewModel>> models =
                new AddMultipleModel<IngridientViewModel, ItemHolderModel<IngridientViewModel>>();

                var ingridients = _context.Ingridients.OrderBy(p => p.IngridientName).Select(p => p).ToList();
                models.itemOneList = ingridients;
                models.itemTwoList = new List<ItemHolderModel<IngridientViewModel>>();

                //Loop creating items and adding them to the right list
                foreach (var item in models.itemOneList)
                {
                    var itemToAdd = new ItemHolderModel<IngridientViewModel>();
                    itemToAdd.heldItem = item;
                    itemToAdd.addItem = false;
                    models.itemTwoList.Add(itemToAdd);
                }

                return View(models);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult CreateIngridientsList(AddMultipleModel<IngridientViewModel, ItemHolderModel<IngridientViewModel>> models)
        {
            if (IsAdmin())
            {
                //Fill empty field "heldItem" from models to match selected ingridients
                var ingridients = _context.Ingridients.FromSqlInterpolated($"SELECT * FROM dbo.Ingridients").ToList();
                int count = 0;
                foreach (var ingridient in ingridients)
                {
                    models.itemTwoList[count].heldItem = ingridient;
                    count++;
                }

                //Get Max ID value to select ID for next Ingridient List
                var result = _context.PizzaIngridients.Max(p => (int?)p.PizzaIngridientListId) ?? 0;
                count = result;

                //3.Do below loop
                foreach (var item in models.itemTwoList)
                {
                    if (item.addItem == true)
                    {
                        PizzaIngridientsViewModel model = new PizzaIngridientsViewModel();
                        model.PizzaIngridientListId = count + 1;
                        model.IngridientId = item.heldItem.Id;
                        _context.PizzaIngridients.Add(model);
                    }
                }
                _context.SaveChanges();

                TempData["message"] = "Succesfully created new Ingridient List!";
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
                //var ingridientsList = _context.Ingridients.Join($"SELECT PizzaIngridients.IngridientId, Ingridients.Id, Ingridients.IngridientName FROM PizzaIngridients INNER JOIN Ingridients ON PizzaIngridients.IngridientId = Ingridients.Id;").ToList();
                var query = from list in _context.PizzaIngridients
                            join i in _context.Ingridients on list.IngridientId equals i.Id
                            orderby list.PizzaIngridientListId, i.DisplayPriority ascending
                            select new
                            {
                                ingridientListId = list.PizzaIngridientListId,
                                ingridientName = i.IngridientName
                            };

                List<IngridientListDetailsModel> ingridients = new List<IngridientListDetailsModel>();

                foreach (var item in query)
                {
                    IngridientListDetailsModel model = new IngridientListDetailsModel();
                    model.IngridientInListId = item.ingridientListId;
                    model.IngridientName = item.ingridientName;
                    ingridients.Add(model);
                }

                return View(ingridients);
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
                var query = from list in _context.PizzaIngridients
                            join i in _context.Ingridients on list.IngridientId equals i.Id
                            select new
                            {
                                ingridientListId = list.PizzaIngridientListId,
                                ingridientName = i.IngridientName
                            };

                List<IngridientListDetailsModel> ingridients = new List<IngridientListDetailsModel>();

                foreach (var item in query)
                {
                    IngridientListDetailsModel model = new IngridientListDetailsModel();
                    model.IngridientInListId = item.ingridientListId;
                    model.IngridientName = item.ingridientName;
                    ingridients.Add(model);
                }

                return View(ingridients);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult DeleteIngridientsList(List<IngridientListDetailsModel> ingridients)
        {
            if (IsAdmin())
            {
                int listId = 0;
                foreach (var item in ingridients)
                {
                    if (listId != item.IngridientInListId)
                    {
                        listId = item.IngridientInListId;
                        var pizzaIngridientsList = _context.PizzaIngridients.FromSqlInterpolated($"Select * from dbo.PizzaIngridients where PizzaIngridientListId = {listId}").ToList();
                        _context.PizzaIngridients.RemoveRange(pizzaIngridientsList);
                    }
                    else
                    {
                        continue;
                    }
                }

                _context.SaveChanges();

                TempData["message"] = "Succesfully created new Menu!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
    }
}
