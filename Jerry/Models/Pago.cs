using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.Models
{
    public class Pago
    {
        [Key]
        public int pagoID { get; set; }

        public int reservacionID { get; set; }
        //A un pago le pertenece solo a una reservacion
        virtual public Reservacion reservacion { get; set; }

        [Required]
        [Display(Name ="Cantidad")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal cantidad { get; set; }

        [Required]
        [Display(Name = "Fecha del pago")]
        [DataType(DataType.Date)]
        public DateTime fechaPago { get; set; }
    }
}