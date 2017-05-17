using Jerry.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.ViewModels
{
    public class ReservacionesViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string nombre { get; set; }

        [Required]
        [Display(Name = "Salon")]
        public string nombreSalon { get; set; }

        [Required]
        [Display(Name = "Inicio del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fechaEventoInicial { get; set; }

        [Required]
        [Display(Name = "Fin del evento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fechaEventoFinal { get; set; }

        [Required]
        [Display(Name = "Detalles")]
        public string Detalles { get; set; }

        [Required]
        [Display(Name = "Costo")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal costo { get; set; }

        [Required]
        public int salonID { get; set; }


        public ReservacionesViewModel() { }
        public ReservacionesViewModel(Reservacion reservacion)
        {
            this.nombre = reservacion.cliente.nombre;
            this.nombreSalon = reservacion.salon.nombre;
            this.fechaEventoInicial = reservacion.fechaEventoInicial;
            this.fechaEventoFinal = reservacion.fechaEventoFinal;
            this.Detalles = reservacion.Detalles;
            this.costo = reservacion.costo;
        }
        public ReservacionesViewModel(List<Reservacion> reservacion)
        {
           foreach(var R in reservacion)
            {
                this.nombre = R.cliente.nombre;
                this.nombreSalon = R.salon.nombre;
                this.fechaEventoInicial = R.fechaEventoInicial;
                this.fechaEventoFinal = R.fechaEventoFinal;
                this.Detalles = R.Detalles;
                this.costo = R.costo;
            }
            
        }

        public static IEnumerable<Jerry.ViewModels.ReservacionesViewModel> ObtenerReservaciones()
        {
            DateTime fechaI = DateTime.Parse("2016/01/25");
            DateTime fechaF = DateTime.Parse("2016/12/26");
            ApplicationDbContext db = new ApplicationDbContext();


            if (fechaI == null || fechaF == null)
            {
                List<Jerry.ViewModels.ReservacionesViewModel> query = new List<Jerry.ViewModels.ReservacionesViewModel>();
                return query;
                
            }
            else
            {

                var query = from R in db.reservaciones.
                      Where(R => R.fechaEventoInicial <= (DateTime)fechaI && R.fechaEventoFinal >= (DateTime)fechaI).ToList()
                          select new ReservacionesViewModel(R);


                return query;                
            }

        }






    }
}