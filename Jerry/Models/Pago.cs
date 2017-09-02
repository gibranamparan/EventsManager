using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.Models
{
    public class Pago
    {
        [Key]
        [DisplayName("No. Pago")]
        public int pagoID { get; set; }

        /// <summary>
        /// Llave foranea de evento asociado
        /// </summary>
        [DisplayName("Evento")]
        public int eventoID { get; set; }
        //A un pago le pertenece solo a una reservacion
        /// <summary>
        /// Evento asociado al pago
        /// </summary>
        virtual public Evento evento { get; set; }

        [Required]
        [Display(Name ="Cantidad")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        [Range(0,double.MaxValue)]
        public decimal cantidad { get; set; }

        [Required]
        [Display(Name = "Fecha del pago")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fechaPago { get; set; }
    }
}