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
            if (IsWorker())
            {
                var Orders = _context.Orders.Where(p => p.OrderConfirmed == false).Select(p => p).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult ConfirmOrder()
        {
            if (IsWorker())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        public IActionResult RejectOrder()
        {
            if (IsWorker())
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult History()
        {
            if (IsWorker())
            {
                return View();
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
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
    }
}
