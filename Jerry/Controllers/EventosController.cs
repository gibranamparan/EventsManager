using Jerry.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static Jerry.Models.Correo;

namespace Jerry.Controllers
{
    [Authorize]
    public class EventosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Evento/Details/:id
        public ActionResult Details(int? id, string errorPagoMsg)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evento res = prepareDetailsView(id, errorPagoMsg);
            if (res == null)
                return HttpNotFound();

            ViewBag.emailConfigured = db.Correos.FirstOrDefault() != null;

            return View(res);
        }

        private Evento prepareDetailsView(int? id, string errorPagoMsg)
        {
            ErrorEmail errorEmail = TempData["errorEmail"] != null
                ? (ErrorEmail)TempData["errorEmail"] : new ErrorEmail();

            Evento res = db.eventos.Find(id);

            ViewBag.errorMail = errorEmail;
            ViewBag.errorPagoMsg = errorPagoMsg;

            return res;
        }

        // GET: Reservacion/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evento res = db.eventos.Find(id);
            if (res == null)
                return HttpNotFound();

            return View(res);
        }

        // POST: Reservacion/Delete/5
        [Authorize(Roles = ApplicationUser.UserRoles.ADMIN + "," + ApplicationUser.UserRoles.ASISTENTE)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evento res = db.eventos.Find(id);
            int clientID = res.clienteID;
            db.eventos.Remove(res);
            db.SaveChanges();
            return RedirectToAction("Details","Clientes", new { id = clientID });
        }
    }
}