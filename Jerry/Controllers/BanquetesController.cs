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

namespace Jerry.Controllers
{
    public class BanquetesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private const string BIND_FIELDS = "banqueteID,fechaBanquete,email,telefono,lugar,descripcionServicio,cantidadPersonas,costo,tipoContrato,clienteID";
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

        public FileResult GenerarContrato(int? id, string tipoContrato)
        {
            Banquete resContrato = db.Banquetes.Find(id);
            Cliente cliente = resContrato.cliente;
            String rutaContrato = "";
            String fechaInicioEvento = resContrato.fechaBanquete.ToLongDateString();
            String horaEvento = resContrato.fechaBanquete.Hour.ToString();
            String lugar = resContrato.lugar;
            if (tipoContrato.Equals(Reservacion.TiposContrato.SERVICIO))
            {
                rutaContrato = "~/App_Data/CONTRATO-de-Prestacion-de-Servicios.docx";
            }
            String nuevoContrato = Server.MapPath("~/App_Data/ContratoEnBlanco.docx");
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath(rutaContrato));
            System.IO.File.WriteAllBytes(nuevoContrato, fileBytesContrato);
            String descripcionServicios = resContrato.descripcionServicio;
            String cantidadPersonas = resContrato.cantidadPersonas.ToString();
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
