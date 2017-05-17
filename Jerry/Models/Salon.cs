using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.Models
{
    public class Salon
    {
        [Key]
        public int salonID { get; set; }

        [Required]
        [Display(Name ="Salon")]
        public string nombre { get; set; }

        [Required]
        [Display(Name ="Detalles")]
        public string detalles { get; set; }

        //Un salon puede tener una coleccion de reservaciones, es decir, puede tener varias reservaciones
        virtual public ICollection<Reservacion> reservaciones { get; set; }
    }
}