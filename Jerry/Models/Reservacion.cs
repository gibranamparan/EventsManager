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
    public class Reservacion : Evento
    {
        /// <summary>
        /// Sesiones en las que se divide la reservación. Contiene un setter que establece automaticamente la fecha
        /// de inicio y fin del evento segun el horario de la sesion inicial y final.
        /// </summary>
        [Display(Name = "Inicio del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm}",
            ApplyFormatInEditMode = true)]
        public virtual ICollection<SesionDeReservacion> sesiones { get; set; }

        public void ajustarFechaInicialFinal()
        {
            if (this.sesiones != null && this.sesiones.Count() > 0)
            {
                this.fechaEventoInicial = this.sesiones.First().periodoDeSesion.startDate;
                this.fechaEventoFinal = this.sesiones.Last().periodoDeSesion.endDate;
            }
        }

        /// <summary>
        /// Arroja una lista lineal de los salones reservados en cada una de las sesiones
        /// para la presente instancia de la reservacion.
        /// </summary>
        [DisplayName("Salón")]
        public string salon
        {
            get
            {
                string res = "";

                if (this.sesiones != null && this.sesiones.Count() > 0)
                {
                    var sesiones = this.sesiones.Select(ses => ses.salon.nombre).Distinct();
                    sesiones.ToList().ForEach(nam => res += nam + ", ");
                    res = res.Trim(' ').Trim(',');
                }

                return res;
            }
        }

        /// <summary>
        /// Genera una lista de informacion donde se muesta el la hora y la fecha de inicio de cada una de las sesiones
        /// asociadas a esta reservacion.
        /// </summary>
        public string sesionesString { get {
                string res = string.Empty;

                //foreach(var ses in this.sesiones) { 
                for (int c = 1; c <= this.sesiones.Count(); c++)
                {
                    var ses = this.sesiones.ElementAt(c - 1);
                    res += ses.stringParaContrato + (c == this.sesiones.Count() - 1 ? " y " : ", ");
                }
                res = res.Trim().TrimEnd(',');

                return res;
            }
        }

        /// <summary>
        /// Arroja una lista de seleccion de los diferentes tipos de contrato para arrendamiento de salones
        /// </summary>
        /// <returns></returns>
        public static List<object> getTipoContratoItemArray()
        {
            List<object> array = new List<object>();
            array.Add(new { Text = Evento.getNombreContrato(TipoDeContrato.ARRENDAMIENTO), Value = TipoDeContrato.ARRENDAMIENTO });
            array.Add(new { Text = Evento.getNombreContrato(TipoDeContrato.KIDS), Value = TipoDeContrato.KIDS });

            return array;
        }

        /// <summary>
        /// Determina el tiempo total que durá la reservacion considerando
        /// todas las sesiones registradas.
        /// </summary>
        public new TimeSpan TiempoTotal
        {
            get
            {
                TimeSpan res = new TimeSpan();
                if (this.sesiones != null && this.sesiones.Count() > 0)
                    foreach (var ses in this.sesiones)
                        res += ses.periodoDeSesion.totalTime;
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
                Where(ses => ses.reservacionID != this.eventoID && (ses.periodoDeSesion.startDate >= this.fechaEventoInicial && ses.periodoDeSesion.startDate <= this.fechaEventoFinal
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

        public override string ToString()
        {
            return String.Format("{0} - {1}", base.ToString(), this.salon);
        }

        /// <summary>
        /// Procedimiento para llenar la plantilla del tipo de contrato B.
        /// </summary>
        /// <param name="res">Registro de reservacion</param>
        /// <param name="data">Datos para rellenar el contrato.</param>
        /// <param name="doc">Instancia de documento word para rellenar.</param>
        public void fillContratoB(VMDataContractReservacion data, ref DocX doc) //CONTRATO MODIFICADO
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
        public void fillContratoA(VMDataContractReservacion data, ref Novacode.DocX doc) //VENTURA KIDS
        {
            doc.ReplaceText("<FECHA>", data.fechaReservacion);
            doc.ReplaceText("<CLIENTE>", data.nombreCliente);
            doc.ReplaceText("<TELEFONO>", data.telefono);
            doc.ReplaceText("<INVITADOS>", data.cantidadPersonas);
            doc.ReplaceText("<DIA>", data.diaEvento);
            doc.ReplaceText("<MES>", data.mesEvento);
            doc.ReplaceText("<AÑO>", data.yearEvento);
            string strConcluye = string.Empty;
            if (this.fechaEventoInicial.Equals(this.fechaEventoFinal))
                strConcluye = "mismo día";
            else
                strConcluye = this.fechaEventoFinal.Day +
                    " DE " + DatesTools.DatesToText.ConvertToMonth(this.fechaEventoFinal, "es")
                    .ToUpperInvariant() + " DEL " + this.fechaEventoFinal.Year;

            doc.ReplaceText("<CONCLUYE>", strConcluye);
            doc.ReplaceText("<HORA_INICIO>", data.horaInicioEvento);
            doc.ReplaceText("<HORA_FIN>", data.horaFinEvento);
            /*doc.ReplaceText("<DURACION>", this.TiempoTotal.Minutes==0?this.TiempoTotal.ToString("hh")
                : this.TiempoTotal.ToString("hh 'con' mm 'minutos'"));*/

            doc.ReplaceText("<LAPSO>", this.sesionesString);
            doc.ReplaceText("<COSTO>", data.costo);
            doc.ReplaceText("<LETRA_TOTAL>", data.costoLetra);

            doc.ReplaceText("<ANTICIPO>", data.anticipo);
            doc.ReplaceText("<DEBE>", data.adeudo);
            doc.ReplaceText("<LETRA_DEUDA>", data.adeudoLetra);
        }

        /// <summary>
        /// Almacena para su vista la informacion de una reservacion, encapsulando el nombre completo del cliente
        /// y una lista de las sesiones que componen la reservacion.
        /// </summary>
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

        /// <summary>
        /// Clase para rellenar los contratos basado en la informacion de la reservación y el cliente.
        /// </summary>
        public class VMDataContractReservacion
        {
            public string descripcionServicios, telefono, correo, fechaReservacion, cantidadPersonas,
            diaEvento, mesEvento, yearEvento, horaInicioEvento, horaFinEvento, costo, costoLetra,
                anticipo, adeudo, adeudoLetra, asociadoCliente, nombreCliente, fechaInicioEvento,
                fechaFinEvento, duracionEvento;

            public VMDataContractReservacion(Reservacion resContrato)
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
                decimal primerPago = resContrato.primerPago!=null?resContrato.primerPago.cantidad:0;
                anticipo = primerPago.ToString();
                adeudo = (resContrato.cantidadFaltante - primerPago).ToString();
                adeudoLetra = NumbersTools.NumberToText.Convert(resContrato.cantidadFaltante, "pesos");
                asociadoCliente = resContrato.cliente.clienteID.ToString();
                nombreCliente = resContrato.cliente.nombreCompleto.ToUpperInvariant();
            }
        }

        /// <summary>
        /// Clase que agrupa los atributos necesarios para mostrar las sesiones que conforman una reservacion en 
        /// elementos graficos sobre un calendario en JavaScript (bootstrap-calendar.js)
        /// </summary>
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
                this.title = string.Format("#Reservacion: {0}, {1}, Cliente: {2}", s1.reservacion.eventoID, s1, s1.reservacion.cliente.nombreCompleto);
                this.url = "/Eventos/Details/" + s1.reservacionID;
                this.start = s1.periodoDeSesion.startDate.ToUniversalTime().Subtract(JS_DATE_REF).TotalMilliseconds;
                this.end = s1.periodoDeSesion.endDate.ToUniversalTime().Subtract(JS_DATE_REF).TotalMilliseconds;
            }
            public ReservacionInScheduleJS(Evento evento)
            {
                this.id = evento.eventoID;
                this.title = string.Format("#Reservacion: {0}, {1}, Cliente: {2}", evento.eventoID, evento, evento.cliente.nombreCompleto);
                this.url = "/Eventos/Details/" + evento.eventoID;
                this.start = evento.fechaEventoInicial.ToUniversalTime().Subtract(JS_DATE_REF).TotalMilliseconds;
                this.end = evento.fechaEventoFinal.ToUniversalTime().Subtract(JS_DATE_REF).TotalMilliseconds;
            }

            /// <summary>
            /// Agrega las sesiones que componen una reservacion en en una lista para ser mostrada 
            /// en bootstrap-calendar.js
            /// </summary>
            /// <param name="res">Reservacion que contiene las sesiones que se desean agregar.</param>
            /// <param name="lista">Argumento por referencia que apunta a la lista donde se desean almacenar las sesiones.</param>
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

    /// <summary>
    /// Representa una sesion que compone una reservación.
    /// </summary>
    public class SesionDeReservacion
    {
        /// <summary>
        /// Llave primaria de la relacion entre reservacion y sesion
        /// </summary>
        [Key]
        public int SesionDeReservacionID { get; set; }

        /// <summary>
        /// Periodo de tiempo asociado al salón
        /// </summary>
        [DisplayName("Sesion")]
        public TimePeriod periodoDeSesion { get; set; }

        [Required]
        [Display(Name = "Salón")]
        /// <summary>
        /// ID del salón asociado
        /// </summary>
        public int salonID { get; set; }

        /// <summary>
        /// Una sesion corresponde unicamente a un salón
        /// </summary>
        public virtual Salon salon { get; set; }

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
        
        /// <summary>
        /// Reservacion asociada a la sesión
        /// </summary>
        [ForeignKey("reservacion")]
        public int reservacionID { get; set; }
        public virtual Reservacion reservacion { get; set; }
        
        public SesionDeReservacion()
        {
            //Por defecto, una sesion tiene un periodo de tiempo
            periodoDeSesion = new TimePeriod();
        }

        /// <summary>
        /// Muestra información de la sesion en el formato "{nombreSalon} - {periodoDeSesion}"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} - {1}", this.salon.nombre, this.periodoDeSesion.ToString());
        }

        /// <summary>
        /// Clase para representar los datos que se verán en vista correpondiente a la clase de sesión
        /// que conforma una reservación.
        /// </summary>
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

