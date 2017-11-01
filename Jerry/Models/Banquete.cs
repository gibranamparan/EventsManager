using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections;
using Novacode;
using System.ComponentModel;

namespace Jerry.Models
{
    public class Banquete: Evento
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name ="Domicilio del Lugar")]
        public string lugar { get; set; }

        [Required]
        [DisplayName("Platillo")]
        public string platillo { get; set;}

        [Required]
        [DisplayName("Tiempos")]
        [Range(1,3,ErrorMessage = "Válido sólo entre uno y tres tiempos.")]
        public int numTiemposPlatillo { get; set; }

        [DisplayName("Platillo")]
        public string platillosInfo { get {
                return String.Format("{0}, {1} tiempos", this.platillo, this.numTiemposPlatillo);
            } }

        public Banquete()
        {
            this.TipoContrato = TipoDeContrato.SERVICIO;
        }

        /// <summary>
        /// Construye una lista de seleccion de contratos que aplican para esta clase de Evento.
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoContratoSelectList(string selVal = Reservacion.TiposContratoNombres.SERVICIO)
        {
            return new SelectList(Banquete.getTipoContratoItemArray(), "Value", "Text", selVal);
        }

        /// <summary>
        /// Arroja una lista de los tipos de contrato que aplican para esta clase de evento Banquetes.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable getTipoContratoItemArray()
        {
            List<object> array = new List<object>();
            array.Add(new { Text = Evento.getNombreContrato(TipoDeContrato.SERVICIO), Value = TipoDeContrato.SERVICIO });
            return array;
        }

        /// <summary>
        /// Procedimiento para llenar la plantilla del tipo de contrato A.
        /// </summary>
        /// <param name="res">Registro de reservacion</param>
        /// <param name="data">Datos para rellenar el contrato.</param>
        /// <param name="doc">Instancia de documento word para rellenar.</param>
        public void fillContratoA(ref Novacode.DocX doc) //BANQUETES SERVICIO
        {
            doc.ReplaceText("<FECHA_ACTUAL>", this.fechaReservacion.ToString("dd/MMMM/yyyy"));
            doc.ReplaceText("<CLIENTE>", this.cliente.nombreCompleto);
            doc.ReplaceText("<FECHA>", this.fechaEventoInicial.ToString("dd/MMMM/yyyy"));
            doc.ReplaceText("<HORA_INICIAL>", this.fechaEventoInicial.ToString("HH:mm'hrs.'"));
            doc.ReplaceText("<TELEFONO>", this.cliente.telefono);
            doc.ReplaceText("<INVITADOS>", this.CantidadPersonas.ToString());
            doc.ReplaceText("<LUGAR>", this.lugar);
            doc.ReplaceText("<PLATILLO>", this.platillosInfo);
            doc.ReplaceText("<COSTO_TOTAL>", this.costo.ToString("C"));
            doc.ReplaceText("<ANTICIPO>", this.primerPago.cantidad.ToString("C"));
            doc.ReplaceText("<DETALLES>", this.Detalles==null?string.Empty:this.Detalles);
            doc.ReplaceText("<LISTA_DE_SERVICIOS>", this.enlistarServiciosParaContrato);
        }

        public List<Banquete> reservacionesQueColisionan(ApplicationDbContext db)
        {
            //Se filtran todas las reservaciones que estan dentro del rango de tiempo total de la reservacion validada
            var resFiltradas = db.Banquetes.Where(ban=>!ban.esCotizacion).
                Where(ses => ses.eventoID != this.eventoID && (ses.fechaEventoInicial >= this.fechaEventoInicial 
                    && ses.fechaEventoInicial <= this.fechaEventoFinal
                || ses.fechaEventoFinal >= this.fechaEventoInicial && ses.fechaEventoFinal <= this.fechaEventoFinal)).ToList();
            
            return resFiltradas.ToList();
        }

        /// <summary>
        /// Almacena para su vista la informacion de banquete, encapsulando el nombre completo del cliente
        /// e informacion sobre el evento
        /// </summary>
        public class VMBanquete
        {
            [DisplayName("#Reservacion de Evento")]
            public int eventoID { get; set; }

            [DisplayName("Cliente")]
            public string nombreCliente { get; set; }

            [DisplayName("Fecha de Inicio")]
            [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
            public DateTime fechaInicial { get; set; }
            [DisplayName("Fecha de Fin")]
            [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}")]
            public DateTime fechaFinal { get; set; }

            public VMBanquete(Banquete res)
            {
                this.eventoID = res.eventoID;
                this.nombreCliente = res.cliente == null ? "" : res.cliente.nombreCompleto;
                this.fechaInicial = res.fechaEventoInicial;
                this.fechaFinal = res.fechaEventoFinal;
            }
        }
    }
}