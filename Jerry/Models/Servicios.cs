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

        [NotMapped]
        public bool fueBorrado { get; set; }
        [NotMapped]
        public int cantidadServicioBorrado { get; set; }

        //TODO: Enlistar solamente servicios para contratos de arrendamiento en la seccion correspondiente
        //TODO: Al igual en banquetes

        public Servicio() { }
        public Servicio(ServiciosEnReservacion ser)
        {
            this.costo = ser.costo;
            this.nombre = ser.nombre;
            this.fueBorrado = true;
            this.cantidadServicioBorrado = ser.cantidad;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}",this.nombre, this.costo);
        }
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
        public int? serviciosID { get; set; }
        public virtual Servicio servicio { get; set; }

        [DisplayName("Nota")]
        [DataType(DataType.MultilineText)]
        public string nota { get; set; }

        [DisplayName("Nombre")]
        public string nombre { get; set; }

        [DisplayName("Cantidad")]
        public int cantidad { get; set; }

        [DisplayName("Monto")]
        [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
        public decimal montoUnitario { get {
                return cantidad * costo;
            } }

        public override string ToString()
        {
            string res = string.Empty;
            string format = (cantidad > 0 ? "{2} " : "") + (String.IsNullOrEmpty(this.nota) ? "{0}" : "{0}: {1}");

            res = string.Format(format, this.nombre, this.nota, this.cantidad);

            return res;
        }
    }
}