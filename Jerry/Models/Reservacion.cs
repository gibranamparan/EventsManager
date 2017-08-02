using Jerry.GeneralTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime fechaReservacion { get; set; }

        [Required]
        [Display(Name = "Inicio del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime fechaEventoInicial { get {
                DateTime res = new DateTime();
                //Si hay sesiones en la reservacion, se toma la fecha inicial de la 1ra sesion
                if(this.sesiones!=null && this.sesiones.Count() > 0)
                {
                    var sesiones = this.sesiones.OrderBy(ses => ses.periodoDeSesion.startDate);
                    res = sesiones.FirstOrDefault().periodoDeSesion.startDate;
                }

                return res;
            }
        }

        [Required]
        [Display(Name = "Fin del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime fechaEventoFinal {
            get
            {
                DateTime res = new DateTime();
                //Si hay sesiones en la reservacion, se toma la fecha inicial de la 1ra sesion
                if (this.sesiones != null && this.sesiones.Count() > 0)
                {
                    var sesiones = this.sesiones.OrderByDescending(ses => ses.periodoDeSesion.endDate);
                    res = sesiones.FirstOrDefault().periodoDeSesion.endDate;
                }
                return res;
            }
        }

        [Required]
        [Display(Name ="Costo Total")]
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
        [Display(Name = "Cantidad de Personas")]
        public int CantidadPersonas { get; set; }

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
        public virtual ICollection<ServiciosEnReservacion> serviciosContratados { get; set; }
        //Sesiones en las que se divide la reservación
        public virtual ICollection<SesionDeReservacion> sesiones { get; set; }

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

        public decimal costoTotalPorServicios
        {
            get
            {
                decimal res = 0;

                if(this.serviciosContratados!=null && this.serviciosContratados.Count()>0)
                {
                    res = this.serviciosContratados.Sum(ser => ser.servicio.costo);
                }
                return res;
            }
        }

        /// <summary>
        /// Verifica si existen reservaciones cuyas sesiones colisionan con las sesiones
        /// de la instancia que se encuentra siendo registrada.
        /// </summary>
        /// <param name="reservacion">Instancia que se encuentra siendo verificada</param>
        /// <returns>Una lista de reservaciones cuyas sesiones colisionan.</returns>
        public static List<Reservacion> reservacionesQueColisionan(Reservacion reservacion)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            DateTime fechaI = reservacion.fechaEventoInicial;
            DateTime fechaF = reservacion.fechaEventoFinal;

            //TODO: Se debe modificar para que verifique por sesiones en salones.
            List<Reservacion> resultado = db.reservaciones.ToList()
                .Where(res => res.fechaEventoInicial <= fechaI && res.fechaEventoFinal >= fechaI
                    || res.fechaEventoInicial <= fechaF && res.fechaEventoFinal >= fechaF).ToList();

            return resultado;

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
            //array.Add(new { Text = "Contrato Prestación de Servicios", Value = Reservacion.TiposContrato.SERVICIO });
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

    public class SesionDeReservacion
    {
        [Key]
        public int SesionDeReservacionID { get; set; }

        [DisplayName("Sesion")]
        public TimePeriod periodoDeSesion { get; set; }

        [ForeignKey("reservacion")]
        public int reservacionID { get; set; }
        public virtual Reservacion reservacion { get; set; }

        public SesionDeReservacion()
        {
            periodoDeSesion = new TimePeriod();
        }
    }
}

