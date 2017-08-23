using Jerry.GeneralTools;
using Novacode;
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
        public TimeSpan TiempoTotal { get {
                TimeSpan res = new TimeSpan();
                if(this.sesiones!=null && this.sesiones.Count() > 0)
                    foreach(var ses in this.sesiones)
                        res += ses.periodoDeSesion.totalTime;
                return res;
            } }

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

        [DisplayName("Salón")]
        public string salon { get {
                string res = "";

                if(this.sesiones!=null && this.sesiones.Count() > 0)
                {
                    var sesiones = this.sesiones.Select(ses => ses.salon.nombre).Distinct();
                    sesiones.ToList().ForEach(nam => res += nam+", ");
                    res = res.Trim(' ').Trim(',');
                }

                return res;
            } }

        [Required]
        [Display(Name = "Cantidad de Personas")]
        public int CantidadPersonas { get; set; }
        
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
        
        /// <summary>
        /// Genera una lista de informacion donde se muesta el la hora y la fecha de inicio de cada una de las sesiones
        /// asociadas a esta reservacion.
        /// </summary>
        public string sesionesString { get {
                string res = string.Empty;

                foreach(var ses in this.sesiones)
                    res += ses.stringParaContrato + ", ";
                res = res.Trim().TrimEnd(',');

                return res;
            } }

        [Display(Name = "Faltante")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadFaltante { get {
                decimal cantidadFaltante = this.costo - this.cantidadPagada;
                return cantidadFaltante;
            } }

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
                if(this.pagos!=null && this.pagos.Count() > 0) { 
                    pagado = this.pagos.Select(c => c.cantidad).Sum();
                }
                return pagado;
            }
        }

        /// <summary>
        /// Determian el total del costo de los servicios seleccionados para esta reservacion.
        /// </summary>
        public decimal costoTotalPorServicios
        {
            get
            {
                decimal res = 0;

                if(this.serviciosContratados!=null && this.serviciosContratados.Count()>0)
                    res = this.serviciosContratados.Sum(ser => ser.servicio.costo);

                return res;
            }
        }

        /// <summary>
        /// Verifica si existen reservaciones cuyas sesiones colisionan con las sesiones
        /// de la instancia que se encuentra siendo registrada.
        /// </summary>
        /// <param name="reservacion">Instancia que se encuentra siendo verificada</param>
        /// <returns>Una lista de reservaciones cuyas sesiones colisionan.</returns>
        public List<SesionDeReservacion> reservacionesQueColisionan(ApplicationDbContext db)
        {
            //Se filtran todas las sesiones que estan dentro del rango de tiempo total de la reservacion validada
            var sesionesFiltradas = db.sesionesEnReservaciones.
                Where(ses => ses.reservacionID!=this.reservacionID && (ses.periodoDeSesion.startDate >= this.fechaEventoInicial && ses.periodoDeSesion.startDate <= this.fechaEventoFinal
                || ses.periodoDeSesion.endDate >= this.fechaEventoInicial && ses.periodoDeSesion.endDate <= this.fechaEventoFinal)).ToList();

            IEnumerable<SesionDeReservacion> res = new List<SesionDeReservacion>();
            IEnumerable<SesionDeReservacion> sesionesConflictuantes = new List<SesionDeReservacion>();
            //Si hay resultados
            if (sesionesFiltradas != null && sesionesFiltradas.Count() > 0)
            {
                //Se verifica si las sesiones de la instancia se traslapan con aquellas en la base de datos
                sesionesConflictuantes = sesiones.SelectMany(ses => sesionesFiltradas
                    .Where(ses2 => ses2.salonID == ses.salonID && ses.periodoDeSesion.hasPartInside(ses2.periodoDeSesion)).ToList());
                res = sesionesConflictuantes;
            }
            return res.ToList();
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
                return new TimePeriod(this.fechaEventoInicial, this.fechaEventoFinal);
            }
        }

        public string enlistarServiciosParaContrato
        {
            get
            { 
                string res = string.Empty;
                foreach(var ser in this.serviciosContratados)
                {
                    res += ser.ToString()+", ";
                }
                res = res.TrimEnd().TrimEnd(',');

                return res;
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
        /// Genera una lista de seleccion de tipos de contrato para ser utilizando en vistas con formas.
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoContratoSelectList(object selVal = null)
        {
            return new SelectList(Reservacion.getTipoContratoItemArray(), "Value", "Text", selVal);
        }
        
        public override string ToString()
        {
            return String.Format("{0} - {1}", this.timePeriod.ToString("dd/MMM hh:mm"), this.salon);
        }


        /// <summary>
        /// Procedimiento para llenar la plantilla del tipo de contrato B.
        /// </summary>
        /// <param name="res">Registro de reservacion</param>
        /// <param name="data">Datos para rellenar el contrato.</param>
        /// <param name="doc">Instancia de documento word para rellenar.</param>
        public void fillContratoB(VMDataContract data, ref DocX doc)
        {
            //Contrato Modificado
            doc.ReplaceText("<CLIENTE>", data.nombreCliente);
            doc.ReplaceText("<SESIONES>", this.sesionesString);
            doc.ReplaceText("<SERVICIOS>", this.enlistarServiciosParaContrato);
            doc.ReplaceText("<TIEMPO>", data.duracionEvento);
            doc.ReplaceText("<FECHA_INICIO>", data.diaEvento);
            doc.ReplaceText("<FECHA_FIN>", data.diaEvento);
            doc.ReplaceText("<DIA>", data.diaEvento);
            doc.ReplaceText("<DIA>", data.diaEvento);
            doc.ReplaceText("<MES>", data.mesEvento);
            doc.ReplaceText("<AÑO>", data.yearEvento);
            doc.ReplaceText("<HORA_INICIO>", data.horaInicioEvento);
            doc.ReplaceText("<HORA_FIN>", data.horaFinEvento);
            doc.ReplaceText("<DIA_FIN>", this.fechaEventoFinal.Day.ToString());
            doc.ReplaceText("<MES_FIN>", DatesTools.DatesToText.ConvertToMonth(this.fechaEventoFinal, "es"));
            doc.ReplaceText("<AÑO_FIN>", this.fechaEventoFinal.Year.ToString());
            doc.ReplaceText("<DESCRIPCION>", data.descripcionServicios);
            doc.ReplaceText("<INVITADOS>", data.cantidadPersonas);
            doc.ReplaceText("<COSTO>", data.costo);
            doc.ReplaceText("<LETRA_TOTAL>", data.costoLetra);
            doc.ReplaceText("<TIEMPO_LETRA>", NumbersTools.NumberToText.Convert(decimal.Parse(data.duracionEvento)).Split(' ')[0]);
            doc.ReplaceText("<DIA_HOY>", DateTime.Today.Day.ToString());
            doc.ReplaceText("<MES_HOY>", DatesTools.DatesToText.ConvertToMonth(DateTime.Today, "es"));
            doc.ReplaceText("<AÑO_HOY>", DateTime.Today.Year.ToString());
        }

        /// <summary>
        /// Procedimiento para llenar la plantilla del tipo de contrato A.
        /// </summary>
        /// <param name="res">Registro de reservacion</param>
        /// <param name="data">Datos para rellenar el contrato.</param>
        /// <param name="doc">Instancia de documento word para rellenar.</param>
        public void fillContratoA(VMDataContract data, ref Novacode.DocX doc)
        {
            doc.ReplaceText("<FECHA>", data.fechaReservacion);
            doc.ReplaceText("<CLIENTE>", data.nombreCliente);
            doc.ReplaceText("<TELEFONO>", data.telefono);
            doc.ReplaceText("<INVITADOS>", data.cantidadPersonas);
            doc.ReplaceText("<DIA>", data.diaEvento);
            doc.ReplaceText("<MES>", data.mesEvento);
            doc.ReplaceText("<AÑO>", data.yearEvento);
            doc.ReplaceText("<HORA_INICIO>", data.horaInicioEvento);
            doc.ReplaceText("<HORA_FIN>", data.horaFinEvento);

            if (this.fechaEventoInicial.Equals(this.fechaEventoFinal))
                doc.ReplaceText("<CONCLUYE>", "mismo día");
            else
                doc.ReplaceText("<CONCLUYE>", this.fechaEventoFinal.Day +
                    " DE " + DatesTools.DatesToText.ConvertToMonth(this.fechaEventoFinal, "es")
                    .ToUpperInvariant() + " DEL " + this.fechaEventoFinal.Year);

            doc.ReplaceText("<COSTO>", data.costo);
            doc.ReplaceText("<LETRA_TOTAL>", data.costoLetra);
            doc.ReplaceText("<ANTICIPO>", data.anticipo);
            doc.ReplaceText("<DEBE>", data.adeudo);
            doc.ReplaceText("<LETRA_DEUDA>", data.adeudoLetra);
        }

        public class VMReservacion{
            [DisplayName("Cliente")]
            public string nombreCliente { get; set; }
            public List<SesionDeReservacion.VMSesion> sesiones { get; set; }

            public VMReservacion(Reservacion res)
            {
                this.nombreCliente = res.cliente==null?"":res.cliente.nombreCompleto;
                this.sesiones = new List<SesionDeReservacion.VMSesion>();
                if(res.sesiones!=null && res.sesiones.Count()>0)
                    res.sesiones.ToList().ForEach(ses => {this.sesiones.Add(new SesionDeReservacion.VMSesion(ses));});
            }
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
        
        public static class TiposContrato
        {
            public const string SERVICIO = "Prestación de Sevicios";
            public const string KIDS = "Ventura Kids";
            public const string EVENTO = "Arrendamiento por Evento";
        }

        /// <summary>
        /// Clase para rellenar los contratos basado en la informacion de la reservación y el cliente.
        /// </summary>
        public class VMDataContract
        {
            public string descripcionServicios, telefono, correo, fechaReservacion, cantidadPersonas,
            diaEvento, mesEvento, yearEvento, horaInicioEvento, horaFinEvento, costo, costoLetra,
                anticipo, adeudo, adeudoLetra, asociadoCliente, nombreCliente, fechaInicioEvento,
                fechaFinEvento, duracionEvento;

            public VMDataContract(Reservacion resContrato)
            {
                fechaInicioEvento = resContrato.fechaEventoInicial.ToShortDateString();
                fechaFinEvento = resContrato.fechaEventoFinal.ToShortDateString();
                duracionEvento = resContrato.TiempoTotal.TotalHours.ToString();
                descripcionServicios = resContrato.Detalles;
                telefono = String.IsNullOrEmpty(resContrato.cliente.telefono) ? string.Empty : resContrato.cliente.telefono;
                correo = String.IsNullOrEmpty(resContrato.cliente.email) ? string.Empty : resContrato.cliente.email;
                fechaReservacion = resContrato.fechaReservacion.ToLongDateString();
                cantidadPersonas = resContrato.CantidadPersonas.ToString();
                diaEvento = resContrato.fechaEventoInicial.Day.ToString();
                mesEvento = DatesTools.DatesToText.ConvertToMonth(resContrato.fechaEventoInicial, "es").ToUpperInvariant();
                yearEvento = resContrato.fechaEventoInicial.Year.ToString();
                horaInicioEvento = resContrato.fechaEventoInicial.Hour.ToString();
                horaFinEvento = resContrato.fechaEventoFinal.Hour.ToString();
                costo = resContrato.costo.ToString();
                costoLetra = NumbersTools.NumberToText.Convert(resContrato.costo, "pesos");
                anticipo = resContrato.cantidadPagada.ToString();
                adeudo = resContrato.cantidadFaltante.ToString();
                adeudoLetra = NumbersTools.NumberToText.Convert(resContrato.cantidadFaltante, "pesos");
                asociadoCliente = resContrato.cliente.clienteID.ToString();
                nombreCliente = resContrato.cliente.nombreCompleto.ToUpperInvariant();
            }
        }

        public class ReservacionInScheduleJS
        {
            public int id { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string @class { get; set; } = "event-info";
            public double start { get; set; }
            public double end { get; set; }

            public readonly static DateTime JS_DATE_REF = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public ReservacionInScheduleJS() { }
            public ReservacionInScheduleJS(SesionDeReservacion s1)
            {
                this.id = s1.SesionDeReservacionID;
                this.title = s1.ToString()+", Cliente: "+s1.reservacion.cliente.nombreCompleto;
                this.url = "/Reservacion/Details/" + s1.reservacionID;
                this.start = s1.periodoDeSesion.startDate.ToUniversalTime().Subtract(JS_DATE_REF).TotalMilliseconds;
                this.end = s1.periodoDeSesion.endDate.ToUniversalTime().Subtract(JS_DATE_REF).TotalMilliseconds;
            }

            public static void ReservacionInSesionesForScheduleJS(Reservacion res, ref List<ReservacionInScheduleJS> lista)
            {
                List<ReservacionInScheduleJS> tempList = new List<ReservacionInScheduleJS>();
                res.sesiones.ToList().ForEach(s1 =>
                {
                    tempList.Add(new ReservacionInScheduleJS(s1));
                });
                lista.AddRange(tempList);
            }
        }
    }

    public class SesionDeReservacion
    {
        [Key]
        public int SesionDeReservacionID { get; set; }

        [DisplayName("Sesion")]
        public TimePeriod periodoDeSesion { get; set; }

        [Required]
        [Display(Name = "Salón")]
        public int salonID { get; set; }
        //Una reservacion es unicamente a un salon
        virtual public Salon salon { get; set; }

        /// <summary>
        /// Muestra la informacion de la sesion en el formato "{duracionSesion} horas comenzando el día 
        /// {dia} del mes de {mes} del año {año}"
        /// </summary>
        public string stringParaContrato { get {
                string res = string.Empty;
                string formatTime = this.periodoDeSesion.totalTime.Minutes>0? "hh con mm" : "hh";
                string time = this.periodoDeSesion.totalTime.ToString(formatTime);
                res = string.Format("{0} horas comenzando el día {1} del mes de {2} del año {3}",
                    time, this.periodoDeSesion.startDate.Day, this.periodoDeSesion.startDate.ToString("MMMM"), 
                    this.periodoDeSesion.startDate.Year);
                return res;
            } }

        //Una sesion esta relacionada con una reservacion
        [ForeignKey("reservacion")]
        public int reservacionID { get; set; }
        public virtual Reservacion reservacion { get; set; }

        //Por defecto, una sesion tiene un periodo de tiempo
        public SesionDeReservacion()
        {
            periodoDeSesion = new TimePeriod();
        }

        /// <summary>
        /// Muestra informacion de la sesion en el formato "{nombreSalon} - {periodoDeSesion}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} - {1}", this.salon.nombre, this.periodoDeSesion.ToString());
        }

        public class VMSesion
        {
            public TimePeriod periodoDeTiempo { get; set; }
            [DisplayName("Salón")]
            public string nombreSalon { get; set; }
            public int salonID { get; set; }
            public bool conflicto = false;
            public VMSesion(SesionDeReservacion ses)
            {
                periodoDeTiempo = ses.periodoDeSesion;
                nombreSalon = ses.salon == null ? "": ses.salon.nombre;
                salonID = ses.salonID;
            }
        }

    }
}

