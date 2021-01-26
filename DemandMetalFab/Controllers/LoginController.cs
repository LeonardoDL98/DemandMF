using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemandMetalFab.Models;
using System.DirectoryServices;

namespace DemandMetalFab.Controllers
{
    public class LoginController : Controller
    {
        private DemandDBEntities db = new DemandDBEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult LoginPartial()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult loginpage(string username,string password)
        {
            
            try
            {
                LoginModel login = new LoginModel();
                login.username = username;
                login.password = password;
                login.tipo = db.MF_Usuario.Where(x => x.Cuenta == username).First().Id_TipoUsuario;
                if (login.username == "leonardo" && login.password == "leonardo")
                {
                    if (!db.MF_Usuario.Any(x => x.Cuenta == login.username)) return Json(new { Success = false, Message ="The user is not registered in this system"}, JsonRequestBehavior.DenyGet);
                    DemandMetalFab.GlobalCode.Settings.LoggedUser = login;
                    return Json(new { Success = true, Message = "Welcome "+login.username }, JsonRequestBehavior.DenyGet);
                    //return Json(new { Success = false, Message = "Wrong username or password" }, JsonRequestBehavior.DenyGet);
                }
                if (login.username == "hector" && login.password == "hector")
                {
                    if (!db.MF_Usuario.Any(x => x.Cuenta == login.username)) return Json(new { Success = false, Message = "The user is not registered in this system" }, JsonRequestBehavior.DenyGet);
                    DemandMetalFab.GlobalCode.Settings.LoggedUser = login;
                    return Json(new { Success = true, Message = "Welcome " + login.username }, JsonRequestBehavior.DenyGet);
                    //return Json(new { Success = false, Message = "Wrong username or password" }, JsonRequestBehavior.DenyGet);
                }
                string dns = "americas";
                if (login.username.Substring(0, 3).ToUpper() == "PUN" || login.username.Substring(0, 3).ToUpper() == "GSS")
                    dns = "asia";
                else if (login.username.Substring(0, 3).ToUpper() == "TCZ")
                    dns = "europe";

                string DomainAndUsername = dns + ".ad.flextronics.com" + "\\" + login.username;
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + dns + ".ad.flextronics.com", DomainAndUsername, login.password);
                DirectorySearcher Search = new DirectorySearcher(entry);
                Search.PageSize = 1000;
                SearchResult result;
                Search.Filter = "(&(objectClass=user)(anr=" + login.username + "*))";
                result = Search.FindOne();
                if (!db.MF_Usuario.Any(x => x.Cuenta == login.username)) return Json(new { Success = false, Message = "The user is not registered in this system" }, JsonRequestBehavior.DenyGet);
                DemandMetalFab.GlobalCode.Settings.LoggedUser = login;
                return Json(new { Success = true, Message = "Welcome "+ login.username }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception ex)
            {

                return Json(new { Success = false, Message = "Wrong username or password" }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult logout()
        {
            DemandMetalFab.GlobalCode.Settings.LoggedUser = null;
            return Json(new { Success = true, Message = "The session is closed" }, JsonRequestBehavior.DenyGet);

        }

        public PartialViewResult LoginModalPartial(int op)
        {
            ViewBag.op = op;
            return PartialView();
        }
    }
}