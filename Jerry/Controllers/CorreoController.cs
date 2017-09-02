using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jerry.Models;
using System.Net.Mail;
using System.IO;
using static Jerry.Models.Correo;
using System.Web.Routing;

namespace Jerry.Controllers
{
    public class CorreoController : Controller
    {
        public const string BIND_FIELDS = "correoID,To,Subject,Body,correoAdmin,contrasena,smtpHost,puertoCorreo,sslEnabled";
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Correo
        [Authorize]
        public async Task<ActionResult> Index()
        {
            return View(await db.Correos.ToListAsync());
        }

        // GET: Correo/Details/5
        [Authorize(Roles =ApplicationUser.UserRoles.ADMIN)]
        public ActionResult Details()
        {
            Correo correo = db.Correos.ToList().FirstOrDefault();
            ViewBag.servicios = db.Servicios.ToList();
            return View(correo);
        }

        // GET: Correo/Create
        [Authorize]
        public ActionResult Create()
        {
            Correo correo = new Correo();
            return View("Form_Correo", correo);
        }

        // POST: Correo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = BIND_FIELDS)] Correo correo)
        {
            if (ModelState.IsValid)
            {
                db.Correos.Add(correo);
                await db.SaveChangesAsync();
                return RedirectToAction("Details");
            }

            return View("Form_Correo",correo);
        }

        // GET: Correo/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Correo correo = await db.Correos.FindAsync(id);
            if (correo == null)
            {
                return RedirectToAction("Create");
            }
            return View("Form_Correo", correo);
        }

        // POST: Correo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = BIND_FIELDS)] Correo correo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(correo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Details");
            }
            return View("Form_Correo", correo);
        }

        // GET: Correo/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Correo correo = await db.Correos.FindAsync(id);
            if (correo == null)
            {
                return HttpNotFound();
            }
            return View(correo);
        }

        // POST: Correo/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Correo correo = await db.Correos.FindAsync(id);
            db.Correos.Remove(correo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
