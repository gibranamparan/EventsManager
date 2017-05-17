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
using Jerry.ViewModels;
namespace Jerry.Controllers
{
    public class ReservacionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: Reservacion}
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var reservaciones = db.reservaciones.Include(r => r.cliente).Include(r => r.salon);
            return View(await reservaciones.ToListAsync());
        }

        // GET: Reservacion/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id)
        {
            decimal pagado;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.reservaciones.FindAsync(id);
            pagado = reservacion.pagos.Select(c => c.cantidad).Sum();
            ViewBag.faltante = String.Format("{0:C}", reservacion.costo - pagado);
            ViewBag.cantidadPagada = String.Format("{0:C}", pagado);

            if (reservacion == null)
            {
                return HttpNotFound();
            }

            return View(reservacion);
        }

        // GET: Reservacion/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre");
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre");
            return View();
        }

        // POST: Reservacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "reservacionID,fechaReservacion,fechaEventoInicial,fechaEventoFinal,costo,Detalles,salonID,clienteID")] Reservacion reservacion)
        {

            if (ModelState.IsValid && Reservacion.validarFecha(reservacion))
            {
                db.reservaciones.Add(reservacion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }


            //ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", reservacion.clienteID);
            ViewBag.id = reservacion.clienteID;
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre", reservacion.salonID);
            //return View(reservacion);
            return RedirectToAction("CreateReservacion", new { id = reservacion.clienteID });
        }

        // GET: Reservacion/Edit/5
        [Authorize]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", reservacion.clienteID);
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre", reservacion.salonID);
            return View(reservacion);
        }

        // POST: Reservacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "reservacionID,fechaReservacion,fechaEventoInicial,fechaEventoFinal,costo,Detalles,salonID,clienteID")] Reservacion reservacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservacion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", reservacion.clienteID);
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre", reservacion.salonID);
            return View(reservacion);
        }

        // GET: Reservacion/Delete/5
        [Authorize]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            return View(reservacion);
        }

        // POST: Reservacion/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Reservacion reservacion = await db.reservaciones.FindAsync(id);
            db.reservaciones.Remove(reservacion);
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
        public ActionResult CreateReservacion(int id)
        {
            ViewBag.clienteID = id;
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre");
            return View();
        }
        public JsonResult ReservacionesConflictivas(/*object fechaI, object fechaF*/)
        {
            DateTime fechaI = DateTime.Parse("2016/01/25");
            DateTime fechaF = DateTime.Parse("2016/12/26");


            //var res = from R in db.reservaciones.
            //          Where(R => R.fechaEventoInicial <= (DateTime)fechaI && R.fechaEventoFinal >= (DateTime)fechaI ||
            //          R.fechaEventoInicial <= (DateTime)fechaF && R.fechaEventoFinal >= (DateTime)fechaF).ToList()
            //          select new ReservacionesViewModel(R);

            //var res = Jerry.ViewModels.ReservacionesViewModel.ObtenerReservaciones();
            var res = from R in db.reservaciones.
                      Where(R => R.fechaEventoInicial <= (DateTime)fechaI && R.fechaEventoFinal >= (DateTime)fechaI ||
                      R.fechaEventoInicial <= (DateTime)fechaF && R.fechaEventoFinal >= (DateTime)fechaF).ToList()
                      select new ReservacionesViewModel(R);

            return Json(res, JsonRequestBehavior.AllowGet);
        }











    }
}
