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
    public class BanquetesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "banqueteID,fechaBanquete,email,telefono,descripcionServicio,cantidadPersonas,costo,tipoContrato,clienteID";
        // GET: Banquetes
        public ActionResult Index()
        {
            var banquetes = db.Banquetes.Include(b => b.cliente);
            return View(banquetes.ToList());
        }

        // GET: Banquetes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banquete banquete = db.Banquetes.Find(id);
            if (banquete == null)
            {
                return HttpNotFound();
            }
            return View(banquete);
        }

        // GET: Banquetes/Create
        public ActionResult Create()
        {
            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre");
            return View();
        }

        // POST: Banquetes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = BIND_FIELDS)] Banquete banquete)
        {
            if (ModelState.IsValid)
            {
                db.Banquetes.Add(banquete);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", banquete.clienteID);
            return View(banquete);
        }

        // GET: Banquetes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banquete banquete = db.Banquetes.Find(id);
            if (banquete == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", banquete.clienteID);
            return View(banquete);
        }

        // POST: Banquetes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = BIND_FIELDS)] Banquete banquete)
        {
            if (ModelState.IsValid)
            {
                db.Entry(banquete).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", banquete.clienteID);
            return View(banquete);
        }

        // GET: Banquetes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banquete banquete = db.Banquetes.Find(id);
            if (banquete == null)
            {
                return HttpNotFound();
            }
            return View(banquete);
        }

        // POST: Banquetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banquete banquete = db.Banquetes.Find(id);
            db.Banquetes.Remove(banquete);
            db.SaveChanges();
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
