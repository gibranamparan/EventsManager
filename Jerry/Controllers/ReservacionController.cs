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

namespace Jerry.Controllers
{
    public class ReservacionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "reservacionID,fechaReservacion,fechaEventoInicial," +
            "fechaEventoFinal,costo,Detalles,salonID,clienteID,TipoContrato,CantidadPersonas";

        // GET: Reservacion}
        [Authorize]
        public ActionResult Index(Reservacion.VMFiltroReservaciones filtroReservaciones)
        {
            //var reservaciones = db.reservaciones.Include(r => r.cliente).Include(r => r.salon).OrderBy(r => r.fechaReservacion);
            TimePeriod periodo = filtroReservaciones.TimePeriod;
            List<Reservacion> reservaciones = new List<Reservacion>();

            DateTime hoyMasMes = DateTime.Today.AddMonths(1).AddDays(1).AddMilliseconds(-1);
            DateTime inicio = new DateTime();
            DateTime fin = new DateTime();

            if (periodo.startDate.ToShortDateString() == DateTime.Today.ToShortDateString() && periodo.endDate.ToShortDateString() == hoyMasMes.ToShortDateString())
            {
                reservaciones = db.reservaciones.ToList().Where(r => r.fechaEventoInicial >= DateTime.Today &&
                r.fechaEventoInicial <= hoyMasMes).OrderByDescending(r => r.fechaEventoInicial).ToList();
            }
            else
            {
                inicio = periodo.startDate.Date;
                fin = periodo.endDate.Date.AddDays(1).AddMilliseconds(-1);
                reservaciones = db.reservaciones.ToList().Where(r => r.fechaEventoInicial >= inicio &&
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
            ViewBag.servicios = db.Servicios.ToList();
            return newReservacion;
        }

        // POST: Reservacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = BIND_FIELDS)]
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
                //Se obtienes todas las reservaciones que colisionan con la que se encuentra registrando
                var colisiones = Reservacion.reservacionesQueColisionan(reservacion);
                if(colisiones.Count() == 0)
                { //Si no hay colisiones se registra
                    //Se registran los servicios relacionados si existen
                    if (serviciosSeleccionados != null && serviciosSeleccionados.Count > 0)
                        reservacion.serviciosContratados = serviciosSeleccionados;
                    //Se registran las sesiones en las que se divide la reservación
                    if (sesionesEnReservacion != null && sesionesEnReservacion.Count > 0)
                        reservacion.sesiones = sesionesEnReservacion;

                    //Guardar registro
                    db.reservaciones.Add(reservacion);
                    numRegs = db.SaveChanges();
                    return RedirectToAction("Details", "Clientes", new { id = reservacion.clienteID });
                }else
                {
                    //Se reporta que hubo colsiones
                    ModelState.AddModelError("", "La fecha seleccionada ya esta ocupada por "+
                        "otras reservaciones ya registradas");
                    ViewBag.colisiones = colisiones;
                }
            }

            //Si llega hasta aca, hubo un problema y se muestra la forma de nuevo
            Reservacion newRes = prepararVista(reservacion.clienteID);
            reservacion.cliente = newRes.cliente;
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

        // POST: Reservacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = BIND_FIELDS)]
        Reservacion reservacion, string listServiciosSeleccionados, string listSesiones)
        {
            int numRegs = 0;
            if (ModelState.IsValid)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();

                //Se eliminan la seleccion de servicios hecha anteriormente
                var serviciosEliminar = db.ServiciosEnReservaciones
                    .Where(ser => ser.reservacionID == reservacion.reservacionID);
                db.ServiciosEnReservaciones.RemoveRange(serviciosEliminar);

                //Se eliminan la sesiones en las que se divide la reservacion
                var sesionesEliminar = db.sesionesEnReservaciones
                    .Where(ser => ser.reservacionID == reservacion.reservacionID);
                db.sesionesEnReservaciones.RemoveRange(sesionesEliminar);

                numRegs = db.SaveChanges(); //Se guardan cambios

                //Se deserializa la lista de servicios seleccionados y sesiones modificadas
                List<ServiciosEnReservacion> serviciosSeleccionados = js.Deserialize<List<ServiciosEnReservacion>>(listServiciosSeleccionados);
                List<SesionDeReservacion> sesionesDeReservacion = js.Deserialize<List<SesionDeReservacion>>(listSesiones);

                //Se asocia nuevamente los servicios y sesiones con la reservacion
                serviciosSeleccionados.ForEach(ser => {
                    ser.reservacionID = reservacion.reservacionID;
                    db.Entry(ser).State = EntityState.Added;
                });
                sesionesDeReservacion.ForEach(ses=> {
                    ses.reservacionID = reservacion.reservacionID;
                    db.Entry(ses).State = EntityState.Added;
                });

                //Se guardan cambios
                db.Entry(reservacion).State = EntityState.Modified;
                numRegs = db.SaveChanges();

                if(numRegs>0) //Si la operacion fue satisfactoria, se redirecciona al historial del cliente
                    return RedirectToAction("Details","Clientes", new { id = reservacion.clienteID });
            }

            //Si llega aqui, es que hubo un error, se muestra nuevamente la forma
            Reservacion newRes = prepararVista(reservacion.clienteID);
            reservacion.cliente = newRes.cliente;
            return View("Form_Reservacion",reservacion);
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

        public FileResult GenerarContrato(int? id, string tipoContrato)
        {
            Reservacion resContrato = db.reservaciones.Find(id);
            Cliente cliente = resContrato.cliente;
            Salon salon = resContrato.salon;
            String rutaContrato = "";
            String fechaInicioEvento = resContrato.fechaEventoInicial.ToShortDateString();
            String fechaFinEvento = resContrato.fechaEventoFinal.ToShortDateString();
            String duracionEvento = (resContrato.fechaEventoFinal - resContrato.fechaEventoInicial).TotalHours.ToString();

            if (tipoContrato.Equals(Reservacion.TiposContrato.EVENTO))
            {
                rutaContrato = "~/App_Data/CONTRATO-MODIFICADO.docx";
            }
            else if (tipoContrato.Equals(Reservacion.TiposContrato.KIDS))
            {
                rutaContrato = "~/App_Data/CONTRATO.VENTURA.KIDs.NEW.docx";
            }
            String nuevoContrato = Server.MapPath("~/App_Data/ContratoEnBlanco.docx");
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath(rutaContrato));
            System.IO.File.WriteAllBytes(nuevoContrato, fileBytesContrato);
            String descripcionServicios = resContrato.Detalles;
            String telefono = cliente.telefono;
            String correo = cliente.email;
            String fechaReservacion = resContrato.fechaReservacion.ToLongDateString();
            String cantidadPersonas = resContrato.CantidadPersonas.ToString();
            String diaEvento = resContrato.fechaEventoInicial.Day.ToString();
            String mesEvento = DatesTools.DatesToText.ConvertToMonth(resContrato.fechaEventoInicial, "es").ToUpperInvariant();
            String yearEvento = resContrato.fechaEventoInicial.Year.ToString();
            String horaInicioEvento = resContrato.fechaEventoInicial.Hour.ToString();
            String horaFinEvento = resContrato.fechaEventoFinal.Hour.ToString();
            String costo = resContrato.costo.ToString();
            String costoLetra = NumbersTools.NumberToText.Convert(resContrato.costo, "pesos");
            String anticipo = resContrato.cantidadPagada.ToString();
            String adeudo = resContrato.cantidadFaltante.ToString();
            String adeudoLetra = NumbersTools.NumberToText.Convert(resContrato.cantidadFaltante, "pesos");

            if (String.IsNullOrEmpty(telefono))
            {
                telefono = "";
            }
            if (String.IsNullOrEmpty(correo))
            {
                correo = "";
            }
            String asociadoCliente = cliente.clienteID.ToString();
            String nombreCliente = cliente.nombreCompleto.ToUpperInvariant();

            var doc = DocX.Load(nuevoContrato);
            if (tipoContrato.Equals(Reservacion.TiposContrato.KIDS))
            {
                doc.ReplaceText("<FECHA>", fechaReservacion);
                doc.ReplaceText("<CLIENTE>", nombreCliente);
                doc.ReplaceText("<TELEFONO>", telefono);
                doc.ReplaceText("<INVITADOS>", cantidadPersonas);
                doc.ReplaceText("<DIA>", diaEvento);
                doc.ReplaceText("<MES>", mesEvento);
                doc.ReplaceText("<AÑO>", yearEvento);
                doc.ReplaceText("<HORA_INICIO>", horaInicioEvento);
                doc.ReplaceText("<HORA_FIN>", horaFinEvento);

                if (fechaInicioEvento.Equals(fechaFinEvento))
                {
                    doc.ReplaceText("<CONCLUYE>", "mismo día");
                }
                else
                {
                    doc.ReplaceText("<CONCLUYE>", resContrato.fechaEventoFinal.Day + " DE " + DatesTools.DatesToText.ConvertToMonth(resContrato.fechaEventoFinal, "es").ToUpperInvariant() + " DEL " + resContrato.fechaEventoFinal.Year);
                }

                doc.ReplaceText("<COSTO>", costo);
                doc.ReplaceText("<LETRA_TOTAL>", costoLetra);
                doc.ReplaceText("<ANTICIPO>", anticipo);
                doc.ReplaceText("<DEBE>", adeudo);
                doc.ReplaceText("<LETRA_DEUDA>", adeudoLetra);
            }
            else if (tipoContrato.Equals(Reservacion.TiposContrato.EVENTO))
            {
                doc.ReplaceText("<CLIENTE>", nombreCliente);
                doc.ReplaceText("<TIEMPO>", duracionEvento);
                doc.ReplaceText("<DIA>", diaEvento);
                doc.ReplaceText("<MES>", mesEvento);
                doc.ReplaceText("<AÑO>", yearEvento);
                doc.ReplaceText("<DESCRIPCION>", descripcionServicios);
                doc.ReplaceText("<HORA_INICIO>", horaInicioEvento);
                doc.ReplaceText("<HORA_FIN>", horaFinEvento);
                doc.ReplaceText("<DIA_FIN>", resContrato.fechaEventoFinal.Day.ToString());
                doc.ReplaceText("<MES_FIN>", DatesTools.DatesToText.ConvertToMonth(resContrato.fechaEventoFinal,"es"));
                doc.ReplaceText("<AÑO_FIN>", resContrato.fechaEventoFinal.Year.ToString());
                doc.ReplaceText("<INVITADOS>", cantidadPersonas);
                doc.ReplaceText("<COSTO>", costo);
                doc.ReplaceText("<LETRA_TOTAL>", costoLetra);
                doc.ReplaceText("<TIEMPO_LETRA>", NumbersTools.NumberToText.Convert(decimal.Parse(duracionEvento)).Split(' ')[0]);
                doc.ReplaceText("<DIA_HOY>", DateTime.Today.Day.ToString());
                doc.ReplaceText("<MES_HOY>", DatesTools.DatesToText.ConvertToMonth(DateTime.Today,"es"));
                doc.ReplaceText("<AÑO_HOY>", DateTime.Today.Year.ToString());
            }

            doc.Save();

            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(nuevoContrato);
            string nombreArchivoDescargado = tipoContrato+"_"+nombreCliente.ToUpperInvariant()+".docx";
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
