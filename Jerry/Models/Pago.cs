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

        public int? reservacionID { get; set; }
        //A un pago le pertenece solo a una reservacion
        virtual public Reservacion reservacion { get; set; }

        public int? banqueteID { get; set; }
        virtual public Banquete banquete { get; set; }

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