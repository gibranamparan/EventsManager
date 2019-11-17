using Jerry.Models;
using Novacode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using static Jerry.Models.Correo;
using static Jerry.Models.Evento;

namespace Jerry.Controllers
{
    [Authorize]
    public class EventosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Evento/Details/:id
        public ActionResult Details(int? id, string errorPagoMsg)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evento res = prepareDetailsView(id, errorPagoMsg);
            if (res == null)
                return HttpNotFound();

            ViewBag.emailConfigured = db.Correos.FirstOrDefault() != null;

            return View(res);
        }

        private Evento prepareDetailsView(int? id, string errorPagoMsg)
        {
            ErrorEmail errorEmail = TempData["errorEmail"] != null
                ? (ErrorEmail)TempData["errorEmail"] : new ErrorEmail();

            Evento res = db.eventos.Find(id);

            ViewBag.errorMail = errorEmail;
            ViewBag.errorPagoMsg = errorPagoMsg;
            ViewBag.correoSettings = db.Correos.FirstOrDefault();

            return res;
        }

        // GET: Reservacion/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evento res = db.eventos.Find(id);
            if (res == null)
                return HttpNotFound();

            return View(res);
        }

        public FileResult descargarContrato(int? id, TipoDeContrato tipoContrato)
        {
            Evento evento = db.eventos.Find(id);
            String rutaContrato = evento.ContratoPath;

            //Se hace una copia una instancia de contrato para ser modificada basada en una plantilla
            String nuevoContrato = Server.MapPath("~/App_Data/ContratoEnBlanco.docx");
            byte[] fileBytesContrato = System.IO.File.ReadAllBytes(Server.MapPath(rutaContrato));
            System.IO.File.WriteAllBytes(nuevoContrato, fileBytesContrato);

            var doc = DocX.Load(nuevoContrato);
            if (evento.tipoDeEvento == TipoEvento.RESERVACION) { //Si el evento es una reservacion de salon por arrendamiento
                Reservacion res = ((Reservacion)evento);
                //Se cargan los datos desde el registro de reservacion
                Reservacion.VMDataContractReservacion dataContracts = new Reservacion.VMDataContractReservacion(res);
                if (tipoContrato == TipoDeContrato.KIDS)//CONTRATO VENTURA KIDS
                    res.fillContratoA(dataContracts, ref doc);
                else if (tipoContrato == TipoDeContrato.ARRENDAMIENTO)//CONTRATO MODIFICADO
                    res.fillContratoB(dataContracts, ref doc);
            }else if(evento.tipoDeEvento == TipoEvento.BANQUETE)
            {
                Banquete res = ((Banquete)evento);
                res.fillContratoA(ref doc); //Contrato para banquetes
            }

            doc.Save(); //Guardar documento en servidor

            //Descargar documento
            byte[] fileBytesNuevoContrato = System.IO.File.ReadAllBytes(nuevoContrato);
            string nombreArchivoDescargado = evento.nombreTipoContrato.Replace(' ','_').ToUpperInvariant() + "_" + evento.cliente.nombreCompleto.ToUpperInvariant();
            nombreArchivoDescargado = Regex.Replace(nombreArchivoDescargado, @"[^a-zA-z0-9 ]+", "") + ".docx";
            return File(fileBytesNuevoContrato, System.Net.Mime.MediaTypeNames.Application.Octet, nombreArchivoDescargado);
        }

        [Authorize(Roles = ApplicationUser.UserRoles.ADMIN + "," + ApplicationUser.UserRoles.ASISTENTE)]
        public ActionResult descargarReporte(int? id, string errorPagoMsg)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evento reservacion = db.eventos.Find(id);

            if (reservacion == null)
                return HttpNotFound();

            var fileView = new Rotativa.ViewAsPdf("ReporteDeEvento", "BlankLayout", reservacion)
            { FileName = reservacion + ".pdf" };

            //Code to get content
            return fileView;
        }

        [Authorize(Roles = ApplicationUser.UserRoles.ADMIN + "," + ApplicationUser.UserRoles.ASISTENTE)]
        public ActionResult verReporte(int? id, string errorPagoMsg)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Evento reservacion = db.eventos.Find(id);

            if (reservacion == null)
                return HttpNotFound();

            /*var fileView = new Rotativa.ViewAsPdf("ReporteDeEvento", "BlankLayout", reservacion)
            { FileName = reservacion + ".pdf" };

            //Code to get content
            return fileView;*/
            return View("ReporteDeEvento", "BlankLayout", reservacion);
        }

        /// <summary>
        /// Send Mail with hotmail
        /// </summary>
        /// <param name="objModelMail">MailModel Object, keeps all properties</param>
        /// <param name="fileUploader">Selected file data, example-filename,content,content type(file type- .txt,.png etc.),length etc.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = ApplicationUser.UserRoles.ADMIN + "," + ApplicationUser.UserRoles.ASISTENTE)]
        public async Task<ActionResult> EnviarCorreo(int? id)
        {
            Evento reservacion = db.eventos.Find(id);
            string clientName = reservacion.cliente.nombreCompleto;
            string bodyDelCorreo = reservacion.descripcionDetallada;
            
            string err = await reservacion.send_eventReport(Request, this.ControllerContext);
            /*RouteValueDictionary rvd = new RouteValueDictionary();
            TempData["errorEmail"] = err;
            rvd.Add("errorEmail", err);*/

            return Json(new { errorMessage = err });
        }

        // POST: Reservacion/Delete/5
        [Authorize(Roles = ApplicationUser.UserRoles.ADMIN + "," + ApplicationUser.UserRoles.ASISTENTE)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evento res = db.eventos.Find(id);
            int clientID = res.clienteID;
            db.eventos.Remove(res);
            db.SaveChanges();
            return RedirectToAction("Details","Clientes", new { id = clientID });
        }
    }
}