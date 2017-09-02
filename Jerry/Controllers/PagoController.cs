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

namespace Jerry.Controllers
{
    public class PagoController : Controller
    {
        public const string BIND_FIELDS = "pagoID,eventoID,cantidad,fechaPago";

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pago
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var pagos = db.pagos.Include(p => p.evento);
            return View(await pagos.ToListAsync());
        }

        // GET: Pago/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = await db.pagos.FindAsync(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            return View(pago);
        }

        // GET: Pago/Create
        [Authorize]
        public ActionResult Create(int id)
        {
            
            ViewBag.reservacionID = id;
            return View();
        }

        // POST: Pago/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = BIND_FIELDS)] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.pagos.Add(pago);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Eventos", new { id = pago.eventoID });
            }

            return RedirectToAction("Details", "Eventos", new { id = pago.eventoID, errorPagoMsg = "Verifique la información introducida"});
        }

        // GET: Pago/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pago pago = await db.pagos.FindAsync(id);
            if (pago == null)
            {
                return HttpNotFound();
            }
            ViewBag.reservacionID = new SelectList(db.reservaciones, "reservacionID", "Detalles", pago.eventoID);
            return View(pago);
        }

        // POST: Pago/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = BIND_FIELDS)] Pago pago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pago).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.reservacionID = new SelectList(db.reservaciones, "reservacionID", "Detalles", pago.eventoID);
            return View(pago);
        }

        // POST: Pago/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = ApplicationUser.UserRoles.ADMIN + ","
            +ApplicationUser.UserRoles.ASISTENTE)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Pago pago = await db.pagos.FindAsync(id);
            int reservacionID = pago.eventoID;
            db.pagos.Remove(pago);
            await db.SaveChangesAsync();
            return RedirectToAction("Details","Reservacion",new { id = reservacionID });
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
