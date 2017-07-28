using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jerry.Models;

namespace Jerry.Controllers
{
    public class ServicioController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Servicio
        public ActionResult Create(Servicio servicio)
        {
            db.Servicios.Add(servicio);
            db.SaveChanges();
            return RedirectToAction("Details","Correo");
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Servicio servicio = db.Servicios.Find(id);
            if (servicio == null)
            {
                return HttpNotFound();
            }
            db.Servicios.Remove(servicio);
            db.SaveChanges();
            return RedirectToAction("Details", "Correo");
        }
    }
}