using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jerry.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Esta Pagina fue elaborada por Netcode";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Quinta Ventura.";

            return View();
        }
    }
}