using Jerry.GeneralTools;
using Jerry.Models;
using Jerry.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Novacode;

namespace Jerry.Controllers
{
    public class ReservacionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "reservacionID,fechaReservacion,fechaEventoInicial," +
            "fechaEventoFinal,costo,Detalles,salonID,clienteID,TipoContrato";

        // GET: Reservacion}
        [Authorize]
        public ActionResult Index(Reservacion.VMFiltroReservaciones filtroReservaciones)
        {
            //var reservaciones = db.reservaciones.Include(r => r.cliente).Include(r => r.salon).OrderBy(r => r.fechaReservacion);
            TimePeriod periodo = filtroReservaciones.TimePeriod;
            List<Reservacion> reservaciones = new List<Reservacion>();

            DateTime hoyMasMes = DateTime.Today.AddMonths(1);
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();

            if (periodo.startDate.ToShortDateString() == DateTime.Today.ToShortDateString())
            {
                reservaciones = db.reservaciones.Include(r => r.cliente).Include(r => r.salon).Where(r => r.fechaEventoInicial >= DateTime.Today &&
                r.fechaEventoInicial <= hoyMasMes).OrderByDescending(r => r.fechaEventoInicial).ToList();
            }
            else
            {
                inicio = periodo.startDate;
                fin = periodo.endDate;
                reservaciones = db.reservaciones.Include(r => r.cliente).Include(r => r.salon).Where(r => r.fechaEventoInicial >= inicio &&
                r.fechaEventoInicial <= fin).OrderByDescending(r => r.fechaEventoInicial).ToList();
            }

            ViewBag.result = reservaciones;

            return View(filtroReservaciones);
        }

        // GET: Reservacion/Details/5
        [Authorize]
        public async Task<ActionResult> Details(int? id, string errorMsg = null)
        {
            Pago pago = new Pago();
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = await db.reservaciones.FindAsync(id);
            
            ViewBag.errorMsg = errorMsg;

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
        public async Task<ActionResult> Create([Bind(Include = BIND_FIELDS)] Reservacion reservacion)
        {

            if (ModelState.IsValid && Reservacion.validarFecha(reservacion))
            {
                db.reservaciones.Add(reservacion);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", "Clientes", new { id = reservacion.clienteID });
            }

            //ViewBag.clienteID = new SelectList(db.clientes, "clienteID", "nombre", reservacion.clienteID);
            ViewBag.id = reservacion.clienteID;
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre", reservacion.salonID);
            //return View(reservacion);
            return RedirectToAction("Details","Clientes", new { id = reservacion.clienteID });
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
        public async Task<ActionResult> Edit([Bind(Include = BIND_FIELDS)] Reservacion reservacion)
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

        public FileResult GenerarContrato(int? id)
        {
            Reservacion resContrato = db.reservaciones.Find(id);
            Cliente cliente = resContrato.cliente;
            Salon salon = resContrato.salon;

            String nuevoContrato = Server.MapPath("~/App_Data/ContratoEnBlanco.docx");
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath("~/App_Data/Contrato.docx"));
            System.IO.File.WriteAllBytes(nuevoContrato, fileBytesContrato);
            String descripcionServicios = resContrato.Detalles;
            String telefono = cliente.telefono;
            String correo = cliente.email;
            if (String.IsNullOrEmpty(telefono))
            {
                telefono = "";
            }
            if (String.IsNullOrEmpty(correo))
            {
                correo = "";
            }
            String asociadoCliente = cliente.clienteID.ToString();
            String nombreCliente = cliente.nombreCompleto;

            var doc = DocX.Load(nuevoContrato);

            doc.ReplaceText("<CLIENTE>", nombreCliente);
            doc.ReplaceText("<DESCRIPCION>", descripcionServicios);
            doc.ReplaceText("<TELEFONO>", telefono);
            doc.ReplaceText("<EMAIL>", correo);

            doc.Save();

            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(nuevoContrato);
            string nombreArchivoDescargado = "Ejemplo_" + DateTime.Now+".docx";
            return File(fileBytesNuevoContrato,System.Net.Mime.MediaTypeNames.Application.Octet,nombreArchivoDescargado);
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
