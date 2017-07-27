using Jerry.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jerry.Models
{
    public class Reservacion
    { 
        [Key]
        public int reservacionID { get; set; }

        [Required]
        [Display(Name = "Fecha de Reservación")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaReservacion { get; set; }

        [Required]
        [Display(Name = "Inicio del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime fechaEventoInicial { get; set; }

        [Required]
        [Display(Name = "Fin del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", 
            ApplyFormatInEditMode = true)]
        public DateTime fechaEventoFinal { get; set; }

        [Required]
        [Display(Name ="Costo")]
        [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
        public decimal costo { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name ="Descripción")]
        public string Detalles { get; set; }

        [Required]
        [Display(Name = "Tipo de Contrato")]
        public string TipoContrato { get; set; }

        [Required]
        [Display(Name ="Salón")]
        public int salonID { get; set; }
        //Una reservacion es unicamente a un salon
        virtual public Salon salon { get; set; }

        [Required]
        [Display(Name ="Cliente")]
        public int clienteID { get; set; }
        //Una reservacion pertenece unicamente a un cliente
        virtual public Cliente cliente { get; set; }

        //Una reservacion puede tener muchos pagos asociados a ella.
        virtual public ICollection<Pago> pagos { get; set; }

        //Servicios seleccionados para esta reservacion
        public ICollection<ServiciosEnReservacion> serviciosContratados { get; set; }

        [Display(Name = "Faltante")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadFaltante { get {
                decimal cantidadFaltante = this.costo - this.cantidadPagada;
                return cantidadFaltante;
            } }

        [Display(Name = "Pagado")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadPagada
        {
            get
            {
                decimal pagado = 0;
                if(this.pagos!=null && this.pagos.Count() > 0) { 
                    pagado = this.pagos.Select(c => c.cantidad).Sum();
                }
                return pagado;
            }
        }

        public static bool validarFecha(Reservacion reservacion)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            DateTime fechaI = reservacion.fechaEventoInicial;
            DateTime fechaF = reservacion.fechaEventoFinal;
            var query = db.reservaciones.Where(res=>res.fechaEventoInicial<=fechaI && res.fechaEventoFinal >= fechaI || res.fechaEventoInicial <= fechaF && res.fechaEventoFinal >= fechaF).Count();
            //var query2= db.reservaciones.Where(res => res.fechaEventoFinal >= fechaI).Count();
            if (query > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public static bool ObtenerReservaciones(object fechaI, object fechaF, out IEnumerable<Jerry.Models.Reservacion> resultado)
        {
            ApplicationDbContext db = new ApplicationDbContext();

    
            if (fechaI==null || fechaF==null)
            {
                List<Jerry.Models.Reservacion> query = new List<Reservacion>();
                resultado = query;
                return false;
            }
            else
            {
                List<Jerry.Models.Reservacion> query = db.reservaciones.Where(res => res.fechaEventoInicial <= (DateTime)fechaI && res.fechaEventoFinal >= (DateTime)fechaI || res.fechaEventoInicial <= (DateTime)fechaF && res.fechaEventoFinal >= (DateTime)fechaF).ToList();
                resultado = query;
                return true;
            }

        }

        [Display(Name = "Periodo")]
        public TimePeriod timePeriod
        {
            get
            {
                return new TimePeriod(this.fechaEventoFinal, this.fechaEventoFinal);
            }
        }

        public static List<object> getTipoContratoItemArray()
        {
            List<object> array = new List<object>();
            array.Add(new { Text = "Contrato Prestación de Servicios", Value = Reservacion.TiposContrato.SERVICIO });
            array.Add(new { Text = "Contrato Por Arrendamiento de Evento", Value = Reservacion.TiposContrato.EVENTO });
            array.Add(new { Text = "Contrato Ventura Kids", Value = Reservacion.TiposContrato.KIDS });

            return array;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoContratoSelectList(object selVal = null)
        {
            return new SelectList(Reservacion.getTipoContratoItemArray(), "Value", "Text", selVal);
        }

        public class VMFiltroReservaciones
        {
            private TimePeriod _timePeriod { get; set; }
            public TimePeriod TimePeriod
            {
                get
                {
                    if(_timePeriod == null)
                    {
                        _timePeriod = new TimePeriod(DateTime.Now, DateTime.Now.AddDays(30));
                    }
                    return _timePeriod;
                }
                set
                {
                    _timePeriod = value;
                }
            }
        }

        public static class TiposContrato
        {
            public const string SERVICIO = "Prestación de Sevicios";
            public const string KIDS = "Ventura Kids";
            public const string EVENTO = "Arrendamiento por Evento";
        }
    }
}

