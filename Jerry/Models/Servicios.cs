using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Jerry.Models.Evento;

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

        [DisplayName("Tipo de Evento")]
        public TipoEvento tipoDeEvento { get; set; }

        //TODO: Enlistar solamente servicios para contratos de arrendamiento en la seccion correspondiente
        //TODO: Al igual en banquetes
    }

    public class ServiciosEnReservacion
    {
        [Key]
        public int id { get; set; }

        [ForeignKey("evento")]
        [DisplayName("Evento")]
        [Required]
        public int eventoID { get; set; }
        public virtual Evento evento { get; set; }

        [DisplayName("Costo")]
        public decimal costo { get; set; }

        [ForeignKey("servicio")]
        [DisplayName("Servicio")]
        [Required]
        public int serviciosID { get; set; }
        public virtual Servicio servicio { get; set; }

        [DisplayName("Nota")]
        [DataType(DataType.MultilineText)]
        public string nota { get; set; }

        [DisplayName("Cantidad")]
        public int cantidad { get; set; }

        public override string ToString()
        {
            string res = string.Empty;
            string format = (cantidad > 0 ? "{2} " : "") + (String.IsNullOrEmpty(this.nota) ? "{0}" : "{0}: {1}");

            res = string.Format(format, this.servicio.nombre, this.nota, this.cantidad);
            return res;
        }
    }
}