using Microsoft.AspNetCore.Mvc;


namespace Pizza2.Models
{
    public class BaseController : Controller
    {
       
        public void SetSessionPrivilages(string username, string privilages)
        {
            HttpContext.Session.SetString("Privilages", privilages);
            HttpContext.Session.SetString("Username", username);
        }

        public int GetSessionPrivilages()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Privilages")))
            {
                return int.Parse(HttpContext.Session.GetString("Privilages"));
            } else
            {
                return -1;
            }
        }

        public string GetSessionUsername()
        {
            if (!String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Privilages")))
            {
                return HttpContext.Session.GetString("Username");
            }
            else
            {
                return "";
            }
        }

        public void UnsetSessionPrivilages()
        {
            HttpContext.Session.Clear();
        }

        public bool IsUser()
        {
            if (GetSessionPrivilages() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsWorker()
        {
            if (GetSessionPrivilages() >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAdmin()
        {
            if(GetSessionPrivilages() >= 3)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
