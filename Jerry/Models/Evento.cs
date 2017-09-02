using Jerry.GeneralTools;
using Novacode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jerry.Models
{
    public class Evento
    {
        [Key]
        public int eventoID { get; set; }

        /// <summary>
        /// Cantidad de personas que se estima que asistiran al evento.
        /// </summary>
        [Required]
        [Display(Name = "Cantidad de Personas")]
        public int CantidadPersonas { get; set; }

        /// <summary>
        /// Llave foranea del cliente que contrató la reservación.
        /// </summary>
        [Required]
        [Display(Name = "Cliente")]
        public int clienteID { get; set; }
        /// <summary>
        /// Cliente que contrata la reservación.
        /// </summary>
        virtual public Cliente cliente { get; set; }

        /// <summary>
        /// Fecha en la que se hizo registro de la reservacion
        /// </summary>
        [Required]
        [Display(Name = "Fecha de Reservación")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaReservacion { get; set; }

        /// <summary>
        /// Fecha agendada para iniciar el evento.
        /// </summary>
        [Required]
        [Display(Name = "Inicio del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime fechaEventoInicial { get; set; }

        /// <summary>
        /// Fecha en la que finaliza el evento.
        /// </summary>
        [Required]
        [Display(Name = "Fin del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime fechaEventoFinal { get; set; }

        /// <summary>
        /// Costo total del evento.
        /// </summary>
        [Required]
        [Display(Name = "Costo Total")]
        [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
        public decimal costo { get; set; }

        /// <summary>
        /// Descripción general del evento.
        /// </summary>
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        public string Detalles { get; set; }

        [Required]
        [Display(Name = "Tipo de Contrato")]
        public TipoDeContrato TipoContrato { get; set; }
        
        /// <summary>
        /// Pagos abonados a la totalidad del costo del evento.
        /// </summary>
        virtual public ICollection<Pago> pagos { get; set; }

        /// <summary>
        /// Servicios seleccionados para esta reservacion
        /// </summary>
        public virtual ICollection<ServiciosEnReservacion> serviciosContratados { get; set; }
        
        /// <summary>
        /// Arroja el nombre del tipo de contrato seleccionado para esta instancia.
        /// </summary>
        public string nombreTipoContrato
        {
            get
            {
                return Evento.getNombreContrato(this.TipoContrato);
            }
        }
        /// <summary>
        /// Arroja el nombre del tipo de contrato seleccionado para esta instancia.
        /// </summary>
        public string nombreTipoEvento
        {
            get
            {
                return Evento.getNombreTipoEvento(this.tipoDeEvento);
            }
        }

        /// <summary>
        /// Por defecto se calcula el tiempo que dura el evento como la resta
        /// de la fecha final con la fecha inicial.
        /// </summary>
        public TimeSpan TiempoTotal {
            get
            {
                return this.timePeriod.totalTime;
            }
        }

        /// <summary>
        /// Instancia que representa el periodo de tiempo que ocupa el evento.
        /// </summary>
        [Display(Name = "Periodo")]
        public TimePeriod timePeriod
        {
            get
            {
                return new TimePeriod(this.fechaEventoInicial, this.fechaEventoFinal);
            }
        }

        /// <summary>
        /// Arroja la cantidad faltante a pagar calculandolo como el total del costo del evento
        /// menos la suma de todos los abonos registrados.
        /// </summary>
        [Display(Name = "Faltante")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadFaltante
        {
            get
            {
                decimal cantidadFaltante = this.costo - this.cantidadPagada;
                return cantidadFaltante;
            }
        }

        /// <summary>
        /// Calcula el total del monto pagado para la reservacion.
        /// </summary>
        [Display(Name = "Pagado")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadPagada
        {
            get
            {
                decimal pagado = 0;
                if (this.pagos != null && this.pagos.Count() > 0)
                {
                    pagado = this.pagos.Select(c => c.cantidad).Sum();
                }
                return pagado;
            }
        }

        /// <summary>
        /// Se determinan los eventos en general que colisionan con la instancia presente
        /// considerando su fecha inicial y fecha final
        /// </summary>
        /// <param name="db">Contexto de la base de datos para hacer busqueda de eventos.</param>
        /// <returns></returns>
        public List<Evento> getEventosQueColisionan(ApplicationDbContext db)
        {
            // TODO: Desarrollar funcionalidad general para deteccion de colisiones.
            return new List<Evento>();
        }

        /// <summary>
        /// Genera una lista de seleccion de tipos de contrato para ser utilizando en vistas con formas.
        /// </summary>
        /// <param name="selVal">Opcional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoContratoSelectList(object selVal = null)
        {
            return new SelectList(Reservacion.getTipoContratoItemArray(), "Value", "Text", selVal);
        }

        /// <summary>
        /// Clase que agrupa una fecha de inicio y fin para filtrar de eventos.
        /// </summary>
        public class VMFiltroEventos
        {
            private TimePeriod _timePeriod { get; set; }
            public TimePeriod TimePeriod
            {
                get
                {
                    if (_timePeriod == null)
                    {
                        //Por defecto un periodo de tiempo se establece desde el dia de hoy hasta 1 mes con la totalidad del dia final
                        _timePeriod = new TimePeriod(DateTime.Today, DateTime.Today.AddMonths(1).AddDays(1).AddMilliseconds(-1));
                    }
                    return _timePeriod;
                }
                set
                {
                    _timePeriod = value;
                }
            }
        }

        /// <summary>
        /// Arroja el primer pago por orden cronoligico registrado para la presente instancia de un evento.
        /// </summary>
        public Pago primerPago
        {
            get
            {
                Pago res = null;

                if (this.pagos != null && this.pagos.Count() > 0)
                    res = this.pagos.OrderBy(p => p.fechaPago).FirstOrDefault();

                return res;
            }
        }

        /// <summary>
        /// Muestra los nombres para vista de cada uno de los tipos de contratos que se manejan para los diferentes tipos de eventos.
        /// </summary>
        public static class TiposContratoNombres
        {
            public const string SERVICIO = "Prestación de Sevicios";
            public const string KIDS = "Ventura Kids";
            public const string ARRENDAMIENTO = "Arrendamiento por Evento";
        }

        /// <summary>
        /// Muestra los nombres para vista de cada uno de los tipos de contratos que se manejan para los diferentes tipos de eventos.
        /// </summary>
        public static class TiposEventoNombres
        {
            public const string BAQUETES = "Servicio de Banquetes";
            public const string RESERVACION= "Evento en Salon Rentado";
            public const string CUALQUIERA = "Cualquier Evento";
        }

        /// <summary>
        /// Se determina el directorio de la plantilla correspondiente al tipo de contrato
        /// </summary>
        /// <param name="tipoContrato">Nombre de tipo de contrato, los cuales son constantes definidos en Reservacion.TiposContrato.</param>
        /// <returns>Cadena de caracteres con la ubicacion de la plantilla.</returns>
        public static string getContratoPath(TipoDeContrato tipoContrato)
        {
            string res = string.Empty;

            if (tipoContrato == TipoDeContrato.ARRENDAMIENTO)
                res = "~/App_Data/CONTRATO_MODIFICADO.docx";
            else if (tipoContrato == TipoDeContrato.KIDS)
                res = "~/App_Data/CONTRATO_VENTURAKIDS.docx";
            else if (tipoContrato == TipoDeContrato.SERVICIO)
                res = "~/App_Data/CONTRATO_SERVICIOS.docx";

            return res;
        }

        /// <summary>
        /// Arroja el nombre para vista segun el tipo de contrato ingreso como argumento
        /// </summary>
        public static string getNombreContrato(TipoDeContrato tipo)
        {
            switch (tipo)
            {
                case TipoDeContrato.ARRENDAMIENTO:
                    return TiposContratoNombres.ARRENDAMIENTO;
                case TipoDeContrato.SERVICIO:
                    return TiposContratoNombres.SERVICIO;
                case TipoDeContrato.KIDS:
                    return TiposContratoNombres.KIDS;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Arroja el nombre para vista segun el tipo de contrato ingreso como argumento
        /// </summary>
        public static string getNombreTipoEvento(TipoEvento tipo)
        {
            switch (tipo)
            {
                case TipoEvento.BANQUETE:
                    return TiposEventoNombres.BAQUETES;
                case TipoEvento.CUALQUIERA:
                    return TiposEventoNombres.CUALQUIERA;
                case TipoEvento.RESERVACION:
                    return TiposEventoNombres.RESERVACION;
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Arroja el enumerador correspindiente al tipo de evento de la presente
        /// instancia.
        /// </summary>
        [DisplayName("Tipo de Evento")]
        public TipoEvento tipoDeEvento
        {
            get {
                TipoEvento res = TipoEvento.CUALQUIERA;
                if (this is Banquete)
                    res = TipoEvento.BANQUETE;
                else if (this is Reservacion)
                    res = TipoEvento.RESERVACION;

                return res;
            }
        }

        /// <summary>
        /// Construye una lista de seleccion de contratos que aplican para esta clase de Evento.
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoEventoSelectList(string selVal = Reservacion.TiposContratoNombres.SERVICIO)
        {
            return new SelectList(Evento.getTipoEventoItemArray(), "Value", "Text", selVal);
        }

        /// <summary>
        /// Arroja una lista de los tipos de contrato que aplican para esta clase de evento Banquetes.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable getTipoEventoItemArray()
        {
            List<object> array = new List<object>();
            array.Add(new { Text = Evento.getNombreTipoEvento(TipoEvento.RESERVACION), Value = TipoEvento.RESERVACION });
            array.Add(new { Text = Evento.getNombreTipoEvento(TipoEvento.BANQUETE), Value = TipoEvento.BANQUETE });
            array.Add(new { Text = Evento.getNombreTipoEvento(TipoEvento.CUALQUIERA), Value = TipoEvento.CUALQUIERA });
            return array;
        }

        public string controllerName
        {
            get
            {
                switch(this.tipoDeEvento){
                    case TipoEvento.BANQUETE:
                        return "Banquetes";
                    case TipoEvento.RESERVACION:
                        return "Reservacion";
                    default:
                        return string.Empty;
                }
            }
        }

        /// <summary>
        /// Enumerador de tipos de contrato
        /// </summary>
        public enum TipoDeContrato
        {
            NONE, SERVICIO, KIDS, ARRENDAMIENTO
        }

        /// <summary>
        /// Enumerador de tipos de evento, ya sea banquete o reservacion de salon.
        /// </summary>
        public enum TipoEvento
        {
            CUALQUIERA, BANQUETE, RESERVACION
        }
    }
}