using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Jerry.Models
{
    public class Servicio
    {
        [Key]
        public int serviciosID { get; set; }

        [DisplayName("Servicio")]
        public string nombre { get; set; }

        [DisplayName("Costo")]
        [DisplayFormat(DataFormatString = "{0:C}", 
            ApplyFormatInEditMode = true)]
        public decimal costo { get; set; }

        [DisplayName("Descripción")]
        [DataType(DataType.MultilineText)]
        public string descripcion { get; set; }

        public ICollection<ServiciosEnReservacion> reservaciones { get; set; }
    }

    public class ServiciosEnReservacion
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("reservacion")]
        [DisplayName("Reservacion")]
        public int reservacionID { get; set; }
        public virtual Reservacion reservacion { get; set; }

        [ForeignKey("servicio")]
        [DisplayName("Servicio")]
        public int? serviciosID { get; set; }
        public virtual Servicio servicio { get; set; }

        [DisplayName("Costo")]
        [DataType(DataType.MultilineText)]
        public string nota { get; set; }
    }
}