using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pizza2.Data;
using Pizza2.Models;

namespace Pizza2.Controllers
{
    public class IngridientsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public IngridientsController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult Index(IngridientViewModel ingridient)
        {
            if (IsAdmin())
            {
                return View();
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult Create( )
        {
            if (IsAdmin())
            {
                TwoItemsModel<IngridientViewModel, string> ingrdientWithPrice = new TwoItemsModel<IngridientViewModel, string>();
                return View( ingrdientWithPrice );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult CreateIngridient(TwoItemsModel<IngridientViewModel, string> itemModel)
        {
            if (IsAdmin())
            {
                if(float.TryParse(itemModel.itemTwo.Replace(".",","), out float itemPrice ))
                {
                    itemModel.itemOne.IngridientPrice = itemPrice;
                    _context.Ingridients.Add( itemModel.itemOne );
                    _context.SaveChanges();
                    SetMessage( "Succesfully created new Ingridient" );
                } else
                {
                    SetErrorMessage( "Couldn't get price of the ingridient..." );
                }

                return RedirectToAction( nameof( Index ), itemModel.itemOne );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult Details( )
        {
            if (IsAdmin())
            {
                //Create a list and fill it by doing querry from Ingridients Table
                //var ingridientsList = _context.Ingridients.FromSqlInterpolated($"SELECT * FROM dbo.Ingridients").ToList();

                var ingridients = _context.Ingridients.OrderBy( p => p.IngridientName ).Select( p => p ).ToList();

                return View( ingridients );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult Delete( )
        {
            if (IsAdmin())
            {
                EditViewModel<IngridientViewModel> models = new EditViewModel<IngridientViewModel>();
                var ingridientsList = _context.Ingridients.FromSqlInterpolated( $"SELECT * FROM dbo.Ingridients" ).ToList();

                models.itemList = ingridientsList;

                return View( models );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult DeleteIngridient(EditViewModel<IngridientViewModel> ingridient)
        {
            if (IsAdmin())
            {
                _context.Ingridients.Attach( ingridient.itemModel );
                _context.Ingridients.Remove( ingridient.itemModel );
                _context.SaveChanges();

                TempData[ "message" ] = "Succesfully deleted ingridient!";
                return RedirectToAction( nameof( Index ) );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult Edit( )
        {
            if (IsAdmin())
            {
                EditViewModel<IngridientViewModel> models = new EditViewModel<IngridientViewModel>();
                var ingridientsList = _context.Ingridients.FromSqlInterpolated( $"SELECT * FROM dbo.Ingridients" ).ToList();

                models.itemList = ingridientsList;

                return View( models );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }

        public IActionResult EditIngridient(EditViewModel<IngridientViewModel> ingridient)
        {
            if (IsAdmin())
            {
                _context.Ingridients.Attach( ingridient.itemModel );
                _context.Ingridients.Update( ingridient.itemModel );
                _context.SaveChanges();

                TempData[ "message" ] = "Succesfully edited ingridients!";
                return RedirectToAction( nameof( Index ) );
            }
            else
            {
                return RedirectToAction( "Login", "Home" );
            }

        }
    }
}
