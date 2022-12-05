using Microsoft.AspNetCore.Mvc;
using Pizza2.Data;
using Pizza2.Models;

namespace Pizza2.Controllers
{
    public class MenuController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
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
                var pizzas = _context.Pizzas.Select(p => p).ToList();
                return View(pizzas);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult CreateMenu(List<PizzaViewModel> model)
        {
            if (IsAdmin())
            {
                //Select highest menuId number + 1
                string name = model[0].PizzaName;
                int highestId;

                if (_context.Menu.Any())
                {
                    highestId = _context.Menu.Max(e => e.menuId);
                    highestId++;
                }
                else
                {
                    highestId = 0;
                }

                for (int i = 0; i < model.Count(); i++)
                {
                    if (model[i].Id == -1)
                    {
                        continue;
                    }
                    else
                    {
                        _context.Menu.Add(new MenuViewModel()
                        {
                            menuId = highestId,
                            IsActive = false,
                            PizzaId = model[i].Id,
                            MenuItemPosition = i,
                            menuName = name
                        });
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
        
        public IActionResult Activate()
        {
            if (IsAdmin())
            {
                List<string> menus = new List<string>();
                if (_context.Menu.Any())
                {
                    menus = _context.Menu.Select(p => p.menuName).Distinct().ToList();
                }
                else
                {
                    menus = new List<string>();
                }

                return View(menus);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult ActivateMenu(List<string> model)
        {
            if (IsAdmin())
            {
                //Deactivate last menu
                var activeMenu = _context.Menu.Where(p => p.IsActive == true).Select(p => p).ToList();
                if (activeMenu.Any())
                {
                    for (int i = 0; i < activeMenu.Count(); i++)
                    {
                        activeMenu[i].IsActive = false;
                    }
                    _context.SaveChanges();

                }


                //Activate selected menu
                string name = model[0];
                var menu = _context.Menu.Where(p => p.menuName == name).Select(p => p).ToList();

                if (menu.Any())
                {
                    for (int i = 0; i < menu.Count(); i++)
                    {
                        menu[i].IsActive = true;
                    }
                    _context.SaveChanges();
                }

                TempData["message"] = "Succesfully activated new Menu!";
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
                List<MenuDetailsModel> model = new List<MenuDetailsModel>();

                //Get necesarry list: distinct menus names, menus, and pizzas
                var menus = _context.Menu.OrderBy(p => p).Select(p => p).ToList();
                var menusNames = _context.Menu.Select(p => p.menuName).Distinct().ToList();
                var pizzas = _context.Pizzas.Select(p => p).ToList();

                //for each distinct name
                foreach (var name in menusNames)
                {
                    MenuDetailsModel x = new MenuDetailsModel();
                    x.PizzaNames = new List<string>();

                    x.MenuName = name;
                    foreach (var menu in menus)
                    {
                        foreach (var pizza in pizzas)
                        {
                            if (menu.PizzaId == pizza.Id && name == menu.menuName)
                            {
                                x.PizzaNames.Add(pizza.PizzaName);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (menu.menuName == name)
                        {
                            x.IsActive = menu.IsActive;
                        }

                    }
                    model.Add(x);
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
                ItemHolderModel<List<MenuViewModel>> model = new ItemHolderModel<List<MenuViewModel>>();
                List<MenuViewModel> menusList = new List<MenuViewModel>();
                var menus = _context.Menu.Where(p => p.IsActive == false).Select(p => p).ToList();

                int lastId = 0;
                for (int i = 0; i < menus.Count(); i++)
                {
                    if (menusList.Count == 0)
                    {
                        lastId = menus[i].menuId;
                        menusList.Add(menus[i]);
                    }

                    if (menus[i].menuId == lastId)
                    {
                        continue;
                    }
                    else if (menus[i].menuId != lastId)
                    {
                        lastId = menus[i].menuId;
                        menusList.Add(menus[i]);
                    }

                }

                model.heldItem = menusList;

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult DeleteMenu(ItemHolderModel<List<MenuViewModel>> model)
        {
            if (IsAdmin())
            {
                var menu = _context.Menu.Where(m => m.menuId == model.itemId).Select(m => m).ToList();
                _context.Menu.RemoveRange(menu);

                _context.SaveChanges();

                TempData["message"] = "Sucessfully removed menu from the database";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        
    }
}
