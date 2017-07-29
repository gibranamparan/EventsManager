using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Jerry.Models
{
    public class Banquete
    {
        [Key]
        public int banqueteID { get; set; }

        [Required]
        [Display(Name = "Fecha del Banquete")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime fechaBanquete { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Correo Eectrónico")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string telefono { get; set; }

        [Required]
        [Display(Name ="Lugar")]
        public string lugar { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción de Servicios")]
        public string descripcionServicio { get; set; }

        [Required]
        [Display(Name = "Cantidad de Personas")]
        public int cantidadPersonas { get; set; }

        [Required]
        [Display(Name = "Costo Banquete")]
        [DisplayFormat(DataFormatString = "{0:C}",
            ApplyFormatInEditMode = true)]
        public decimal costo { get; set; }

        [Required]
        [Display(Name = "Tipo de Contrato")]
        public string tipoContrato { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int clienteID { get; set; }
        //Un banquete pertenece unicamente a un cliente
        virtual public Cliente cliente { get; set; }

        //Un banquete puede tener muchos pagos asociados a él.
        virtual public ICollection<Pago> pagos { get; set; }

        [Display(Name = "Faltante")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadFaltante
        {
            get
            {
                decimal cantidadFaltante = this.costo - this.cantidadPagada;
                return cantidadFaltante;
            }
        }

        [Display(Name = "Pagado")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
        public decimal cantidadPagada
        {
            get
            {
                decimal pagado = 0;
                if (this.pagos != null && this.pagos.Count() > 0)
                {
                    pagado = this.pagos.Select(c => c.cantidad).Sum();
                }
                return pagado;
            }
        }

        public static List<object> getTipoContratoItemArray()
        {
            List<object> array = new List<object>();
            array.Add(new { Text = "Contrato Prestación de Servicios", Value = Reservacion.TiposContrato.SERVICIO });

            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoContratoSelectList(string selVal = Reservacion.TiposContrato.SERVICIO)
        {
            return new SelectList(Banquete.getTipoContratoItemArray(), "Value", "Text", selVal);
        }
    }
}