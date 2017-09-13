using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jerry.Models;
using Jerry.GeneralTools;
using Novacode;
using System.Web.Script.Serialization;
using static Jerry.Models.Reservacion;

namespace Jerry.Controllers
{
    public class BanquetesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "eventoID,fechaReservacion,fechaEventoInicial,fechaEventoFinal," +
            "costo,lugar,tipoContrato,clienteID,Detalles,listServiciosSeleccionados,numTiemposPlatillo,"+
            "platillo,CantidadPersonas,totalPorServicios";
        // GET: Banquetes
        public ActionResult Index(Reservacion.VMFiltroEventos filtroReservaciones, bool listMode = true)
        {
            List<Banquete> reservaciones = filterReservaciones(filtroReservaciones);
            ViewBag.result = reservaciones;
            ViewBag.listMode = listMode;
            return View(filtroReservaciones);
        }

        private List<Banquete> filterReservaciones(Evento.VMFiltroEventos filtroReservaciones)
        {
            //Filtra los eventos cuyo inicio estan dentro del filtro 
            TimePeriod periodo = filtroReservaciones.TimePeriod;
            var res = db.Banquetes.ToList()
                .Where(s => periodo.hasInside(s.timePeriod.startDate) || periodo.hasInside(s.timePeriod.endDate)).ToList();
            return res;
        }

        [AllowAnonymous]
        public JsonResult apiReservacionesFilter(string from, string to)
        {
            //Se prepara el filtro para buscar las reservaciones
            Reservacion.VMFiltroEventos filtroReservaciones = new Reservacion.VMFiltroEventos();
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                DateTime dtRef = Reservacion.ReservacionInScheduleJS.JS_DATE_REF;
                //El cliente esta enviando hora con GMT -7, se corrige
                //TODO: ¿De que manera es posible indicarle al calendario que la fecha debe enviarse en formato universal
                DateTime startDate = dtRef.AddMilliseconds(double.Parse(from)).AddHours(-7);
                DateTime endDate = dtRef.AddMilliseconds(double.Parse(to)).AddHours(-7);
                filtroReservaciones.TimePeriod = new TimePeriod(startDate, endDate);
            }

            var reservaciones = filterReservaciones(filtroReservaciones);
            List<Reservacion.ReservacionInScheduleJS> itemsForSchedule = new List<Reservacion.ReservacionInScheduleJS>();
            reservaciones.ToList().ForEach(res =>
            {itemsForSchedule.Add(new ReservacionInScheduleJS(res));});

            return Json(new { success = 1, result = itemsForSchedule }, JsonRequestBehavior.AllowGet);

        }

        // GET: Banquetes/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Banquete ban = prepararVista(id.Value);

            return View("Form_Banquete", ban);
        }

        private Banquete prepararVista(int clienteID = 0)
        {
            Banquete newReservacion = new Banquete();
            Cliente cliente = db.clientes.Find(clienteID);
            newReservacion.clienteID = clienteID;
            newReservacion.cliente = cliente;
            ViewBag.servicios = db.Servicios.Where(s => s.tipoDeEvento == Evento.TipoEvento.BANQUETE).ToList();
            return newReservacion;
        }

        // POST: Banquetes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = BIND_FIELDS)] Banquete banquete, string listServiciosSeleccionados)
        {
            //Se deserializa la lista de compras en un objeto
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ServiciosEnReservacion> serviciosSeleccionados = js.Deserialize<List<ServiciosEnReservacion>>(listServiciosSeleccionados);
            //Se registran los servicios relacionados si existen
            if (serviciosSeleccionados != null && serviciosSeleccionados.Count > 0)
                banquete.serviciosContratados = serviciosSeleccionados;
            if (ModelState.IsValid)
            {
                banquete.fechaReservacion = DateTime.Now;
                db.Banquetes.Add(banquete);
                db.SaveChanges();
                return RedirectToAction("Details", "Clientes", new { id = banquete.clienteID });
            }

            //Si llega hasta aca, hubo un problema y se muestra la forma de nuevo
            Banquete newRes = prepararVista(banquete.clienteID);
            banquete.cliente = newRes.cliente;
            ViewBag.failPostBack = true;
            return View("Form_Banquete", banquete);
        }

        [Authorize]
        [HttpPost]
        public JsonResult checarConflictos(Banquete banquete)
        {
            //Se encuentran las reservaciones en conflicto
            List<Banquete> resFiltradas = banquete.reservacionesQueColisionan(db).ToList();

            //Se prepara la informacion para ser respondida como vista en JSON
            var resultado = (from res in resFiltradas
                             select new Banquete.VMBanquete(res)).ToList();

            var vmReservacionComprobada = new Banquete.VMBanquete(banquete);            

            return Json(resultado, JsonRequestBehavior.AllowGet);
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
            prepararVista();
            return View("Form_Banquete", banquete);
        }

        // POST: Banquetes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = BIND_FIELDS)] Banquete banquete, string listServiciosSeleccionados)
        {
            int numRegs = 0;
            if (ModelState.IsValid)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                //Se eliminan la seleccion de servicios hecha anteriormente
                var serviciosEliminar = db.ServiciosEnReservaciones
                    .Where(ser => ser.eventoID == banquete.eventoID);
                db.ServiciosEnReservaciones.RemoveRange(serviciosEliminar);

                numRegs = db.SaveChanges(); //Se guardan cambios
                //Se deserializa la lista de servicios seleccionados y sesiones modificadas
                List<ServiciosEnReservacion> serviciosSeleccionados = js.Deserialize<List<ServiciosEnReservacion>>(listServiciosSeleccionados);

                //Se asocia nuevamente los servicios con el evento
                serviciosSeleccionados.ForEach(ser => {
                    ser.eventoID = banquete.eventoID;
                    db.Entry(ser).State = EntityState.Added;
                });

                db.Entry(banquete).State = EntityState.Modified;
                numRegs = db.SaveChanges();

                if (numRegs > 0) //Si la operacion fue satisfactoria, a los detalles del eventos
                    return RedirectToAction("Details","Eventos", new { id = banquete.eventoID });
            }
            //Si llega aqui, es que hubo un error, se muestra nuevamente la forma
            Banquete newRes = prepararVista(banquete.clienteID);
            banquete.cliente = newRes.cliente;
            return View("Form_Banquete", banquete);
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
