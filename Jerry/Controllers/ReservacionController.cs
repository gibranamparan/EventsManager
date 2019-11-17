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
using System.Globalization;
using System.Web.Script.Serialization;
using Rotativa.Options;
using static Jerry.Models.Correo;
using System.Web.Routing;
using System.IO;
using static Jerry.Models.Evento;

namespace Jerry.Controllers
{
    public class ReservacionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "eventoID,fechaReservacion,fechaEventoInicial,totalPorServicios," +
            "fechaEventoFinal,costo,Detalles,salonID,clienteID,TipoContrato,CantidadPersonas,esCotizacion,platillo";

        // GET: Reservacion}
        [Authorize]
        public ActionResult Index(Reservacion.VMFiltroEventos filtroReservaciones, bool listMode = false, bool soloCotizaciones = false)
        {
            var reservaciones = filterReservaciones(filtroReservaciones, soloCotizaciones);
            ViewBag.result = reservaciones;
            ViewBag.listMode = listMode;
            ViewBag.soloCotizaciones = soloCotizaciones;

            return View(filtroReservaciones);
        }

        [AllowAnonymous]
        public JsonResult apiReservacionesFilter(string from, string to)
        {
            //Se prepara el filtro para buscar las reservaciones
            Reservacion.VMFiltroEventos filtroReservaciones = new Reservacion.VMFiltroEventos();
            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to)) { 
                DateTime dtRef = Reservacion.ReservacionInScheduleJS.JS_DATE_REF;
                //El cliente esta enviando hora con GMT -7, se corrige
                //TODO: ¿De que manera es posible indicarle al calendario que la fecha debe enviarse en formato universal
                DateTime startDate = dtRef.AddMilliseconds(double.Parse(from)).AddHours(-7);
                DateTime endDate = dtRef.AddMilliseconds(double.Parse(to)).AddHours(-7);
                filtroReservaciones.TimePeriod = new TimePeriod(startDate, endDate);
            }

            var reservaciones = filterReservaciones(filtroReservaciones);
            List<Reservacion.ReservacionInScheduleJS> itemsForSchedule = new List<Reservacion.ReservacionInScheduleJS>();
            reservaciones.ToList().ForEach(res => Reservacion.ReservacionInScheduleJS.ReservacionInSesionesForScheduleJS(res, ref itemsForSchedule));

            return Json(new { success = 1, result=itemsForSchedule }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Busca las reservaciones cuyas sesiones entran dentro del rango de tiempo indicado.
        /// </summary>
        /// <param name="filtroReservaciones">Objeto que representa el rango de tiempo de busqueda.</param>
        /// <returns></returns>
        private IEnumerable<Reservacion> filterReservaciones(Reservacion.VMFiltroEventos filtroReservaciones, bool soloCotizaciones = false)
        {
            TimePeriod periodo = filtroReservaciones.TimePeriod;
            List<Reservacion> reservaciones = new List<Reservacion>();

            //Se seleccionan las reservaciones cuyas sesiones tienen horarios de inicio o fin dentro del rango de busqueda
            reservaciones = db.reservaciones.Where(res => res.esCotizacion == soloCotizaciones).SelectMany(res=>res.sesiones)
                .Where(s => s.periodoDeSesion.startDate >= periodo.startDate && s.periodoDeSesion.startDate <= periodo.endDate
                || s.periodoDeSesion.endDate >= periodo.startDate && s.periodoDeSesion.endDate <= periodo.endDate)
                .Select(s=>s.reservacion).Distinct().ToList();
            //Se ordenan cronologicamente por fecha de inicio
            reservaciones = reservaciones.OrderByDescending(r => r.fechaEventoInicial).ToList();

            return reservaciones;
        }

        // GET: Reservacion/Create
        [Authorize]
        public ActionResult Create(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Reservacion newReservacion = prepararVista(id);
            return View("Form_Reservacion", newReservacion);
        }

        private Reservacion prepararVista(int clienteID=0)
        {
            Reservacion newReservacion = new Reservacion();
            Cliente cliente = db.clientes.Find(clienteID);
            newReservacion.clienteID = clienteID;
            newReservacion.cliente = cliente;
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre");
            /*ViewBag.servicios = db.Servicios.Where(s=>s.tipoDeEvento == TipoEvento.RESERVACION
                || s.tipoDeEvento == Evento.TipoEvento.CUALQUIERA).ToList();*/
            ViewBag.servicios = db.Servicios.ToList().OrderBy(s=>s.tipoDeEvento).ToList();
            return newReservacion;
        }

        // POST: Reservacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Exclude = "fechaEventoInicial, fechaEventoFinal",Include = BIND_FIELDS)]
        Reservacion reservacion, string listServiciosSeleccionados, string listSesiones)
        {
            //Se deserializa la lista de compras en un objeto
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ServiciosEnReservacion> serviciosSeleccionados = js.Deserialize<List<ServiciosEnReservacion>>(listServiciosSeleccionados);
            List<SesionDeReservacion> sesionesEnReservacion = js.Deserialize<List<SesionDeReservacion>>(listSesiones);
            int numRegs = 0;
            //Si la informacion es valida y no hay colisiones
            if (ModelState.IsValid)
            {
                //Se registran las sesiones en las que se divide la reservación
                if (sesionesEnReservacion != null && sesionesEnReservacion.Count > 0)
                    reservacion.sesiones = sesionesEnReservacion;
                
                //Se registran los servicios relacionados si existen
                if (serviciosSeleccionados != null && serviciosSeleccionados.Count > 0)
                    reservacion.serviciosContratados = serviciosSeleccionados.Where(ser=>ser.cantidad >0).ToList();

                //Se obtienes todas las reservaciones que colisionan con la que se encuentra registrando
                var colisiones = reservacion.reservacionesQueColisionan(db);

                //Si no hay colisiones
                if(colisiones.Count() == 0)
                { //Si no hay colisiones se registra
                    reservacion.fechaReservacion = DateTime.Now;
                    reservacion.ajustarFechaInicialFinal(); //Se establece como fecha inicial y final las fechas de las sesiones.
                    //Guardar registro
                    db.reservaciones.Add(reservacion);
                    numRegs = db.SaveChanges();
                    //return RedirectToAction("Details", "Clientes", new { id = reservacion.clienteID });
                    return RedirectToAction("Details", "Eventos", new { id = reservacion.eventoID });
                }
                else
                {
                    //Se reporta que hubo colsiones
                    ModelState.AddModelError("", "La fecha seleccionada ya esta ocupada por "+
                        "otras reservaciones ya registradas");

                    //Se carga la informacion relacioada con los servicios ya seleccionados
                    foreach (var ser in reservacion.serviciosContratados) {
                        db.Entry(ser).State = EntityState.Added;
                        db.Entry(ser).Reference(s => s.servicio).Load();
                    }

                    ViewBag.colisiones = colisiones;
                }
            }

            //Si llega hasta aca, hubo un problema y se muestra la forma de nuevo
            Reservacion newRes = prepararVista(reservacion.clienteID);
            reservacion.cliente = newRes.cliente;
            ViewBag.failPostBack = true;
            return View("Form_Reservacion",reservacion);
        }

        // GET: Reservacion/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservacion reservacion = db.reservaciones.Find(id);
            if (reservacion == null)
            {
                return HttpNotFound();
            }
            prepararVista();
            return View("Form_Reservacion",reservacion);
        }

        [Authorize]
        [HttpPost]
        public JsonResult checarConflictos(Reservacion reservacion)
        {
            //Se ajustan las fechas iniciales y finales del total de la reservacion a la sesion inicial y final
            reservacion.ajustarFechaInicialFinal();
            //Se encuentran las reservaciones y sesiones en conflicto
            var sesionesConConflictos = reservacion.reservacionesQueColisionan(db).ToList();
            var reservacionesConflictos = sesionesConConflictos.Select(ses => ses.reservacion).Distinct().ToList();
            //Se prepara la informacion para ser respondida como vista en JSON
            var resultado = (from res in reservacionesConflictos
                            select new Reservacion.VMReservacion(res)).ToList();

            var vmReservacionComprobada = new Reservacion.VMReservacion(reservacion);
            //Dentro del resultado, se marca cada una de las sesiones que estan causando conflicto

            //Se marcan los conflictos en cada una de las sesiones
            vmReservacionComprobada.sesiones.ForEach(s2 => //Sesiones consultadas
            {
                resultado.ForEach(
                    res => res.sesiones.ForEach(s1 => //Sesiones con conflicto encontradas
                    {
                        if (!s1.conflicto) { 
                            s1.conflicto = s2.periodoDeTiempo.hasPartInside(s1.periodoDeTiempo) && s1.salonID == s2.salonID;
                        }
                    }));
            });

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        // POST: Reservacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "fechaEventoInicial, fechaEventoFinal", Include = BIND_FIELDS)]
        Reservacion reservacion, string listServiciosSeleccionados, string listSesiones)
        {
            int numRegs = 0;
            if (ModelState.IsValid)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();

                //Se eliminan la seleccion de servicios hecha anteriormente
                var serviciosEliminar = db.ServiciosEnReservaciones
                    .Where(ser => ser.eventoID == reservacion.eventoID);
                db.ServiciosEnReservaciones.RemoveRange(serviciosEliminar);

                //Se eliminan la sesiones en las que se divide la reservacion
                var sesionesEliminar = db.sesionesEnReservaciones
                    .Where(ser => ser.reservacionID == reservacion.eventoID);
                db.sesionesEnReservaciones.RemoveRange(sesionesEliminar);

                numRegs = db.SaveChanges(); //Se guardan cambios

                //Se deserializa la lista de servicios seleccionados y sesiones modificadas
                List<ServiciosEnReservacion> serviciosSeleccionados = js.Deserialize<List<ServiciosEnReservacion>>(listServiciosSeleccionados);
                List<SesionDeReservacion> sesionesDeReservacion = js.Deserialize<List<SesionDeReservacion>>(listSesiones);

                //Se asocia nuevamente los servicios y sesiones con la reservacion
                serviciosSeleccionados.ForEach(ser => {
                    ser.eventoID = reservacion.eventoID;
                    db.Entry(ser).State = EntityState.Added;
                });
                sesionesDeReservacion.ForEach(ses=> {
                    ses.reservacionID = reservacion.eventoID;
                    db.Entry(ses).State = EntityState.Added;
                });
                reservacion.sesiones = sesionesDeReservacion;
                reservacion.ajustarFechaInicialFinal();
                //Se guardan cambios
                db.Entry(reservacion).State = EntityState.Modified;
                numRegs = db.SaveChanges();

                if(numRegs>0) //Si la operacion fue satisfactoria, se redirecciona al historial del cliente
                    return RedirectToAction("Details","Eventos", new { id = reservacion.eventoID});
            }

            //Si llega aqui, es que hubo un error, se muestra nuevamente la forma
            Reservacion newRes = prepararVista(reservacion.clienteID);
            reservacion.cliente = newRes.cliente;
            return View("Form_Reservacion",reservacion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }

        public ActionResult CreateReservacion(int id)
        {
            ViewBag.clienteID = id;
            ViewBag.salonID = new SelectList(db.salones, "salonID", "nombre");
            return View();
        }
    }
}
