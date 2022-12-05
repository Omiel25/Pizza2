using Microsoft.AspNetCore.Mvc;
using Pizza2.Data;
using Pizza2.Models;

namespace Pizza2.Controllers
{
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index()
        {
            if (IsAdmin())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult Create()
        {
            if (IsAdmin())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public IActionResult CreateUser(UserViewModel user)
        {
            if (IsAdmin())
            {
                if (!String.IsNullOrWhiteSpace(user.UserName) && !String.IsNullOrWhiteSpace(user.Password))
                {
                    _context.User.Add(user);
                }

                _context.SaveChanges();

                TempData["message"] = "Succesfully created the user!";
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
                var users = _context.User.Where(u => u.Privilages <= 1).Select(u => u).ToList();
                return View(users);
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
                var users = _context.User.Where(u => u.Privilages <= 1).Select(u => u).ToList();
                return View(users);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult DeleteUser(List<UserViewModel> model)
        {
            if (IsAdmin())
            {
                var user = _context.User.Where(u => u.Id == model[0].Id).Single();

                _context.User.Remove(user);
                _context.SaveChanges();

                TempData["message"] = "Succesfully removed user form the database!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
    }
}
