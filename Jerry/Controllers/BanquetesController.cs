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

namespace Jerry.Controllers
{
    public class BanquetesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "eventoID,fechaReservacion,fechaEventoInicial,fechaEventoFinal," +
            "costo,lugar,tipoContrato,clienteID,Detalles,listServiciosSeleccionados,CantidadPersonas";
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

        public FileResult GenerarContrato(int? id, string tipoContrato)
        {
            Banquete resContrato = db.Banquetes.Find(id);
            Cliente cliente = resContrato.cliente;
            String rutaContrato = "";
            String fechaInicioEvento = resContrato.fechaEventoFinal.ToLongDateString();
            String horaEvento = resContrato.fechaEventoFinal.Hour.ToString();
            String lugar = resContrato.lugar;
            if (tipoContrato.Equals(Reservacion.TiposContratoNombres.SERVICIO))
            {
                rutaContrato = "~/App_Data/CONTRATO-de-Prestacion-de-Servicios.docx";
            }
            String nuevoContrato = Server.MapPath("~/App_Data/ContratoEnBlanco.docx");
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath(rutaContrato));
            System.IO.File.WriteAllBytes(nuevoContrato, fileBytesContrato);
            String descripcionServicios = resContrato.Detalles;
            String cantidadPersonas = resContrato.CantidadPersonas.ToString();
            String costo = resContrato.costo.ToString();
            String anticipo = resContrato.cantidadPagada.ToString();
            String adeudo = resContrato.cantidadFaltante.ToString();
            String asociadoCliente = cliente.clienteID.ToString();
            String nombreCliente = cliente.nombreCompleto.ToUpperInvariant();

            var doc = DocX.Load(nuevoContrato);
            doc.ReplaceText("<FECHA>", DateTime.Today.ToLongDateString());
            doc.ReplaceText("<CLIENTE>", nombreCliente);
            doc.ReplaceText("<FECHA_EVENTO>", fechaInicioEvento);            
            doc.ReplaceText("<HORA>", horaEvento);
            doc.ReplaceText("<INVITADOS>", cantidadPersonas);
            doc.ReplaceText("<LUGAR>", lugar);
            doc.ReplaceText("<COSTO>", costo);
            //TODO cambiar anticipo por un valor que indique la cantidad de anticipo
            doc.ReplaceText("<ANTICIPO>", anticipo);
            doc.ReplaceText("<ABONOS>", anticipo);
            doc.ReplaceText("<DESCRIPCION>", descripcionServicios);

            doc.Save();

            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(nuevoContrato);
            string nombreArchivoDescargado = tipoContrato + "_" + nombreCliente.ToUpperInvariant() + ".docx";
            return File(fileBytesNuevoContrato, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivoDescargado);
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
