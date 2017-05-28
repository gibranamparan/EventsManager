using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Descripción de Servicios")]
        public string descripcionServicio { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        public int clienteID { get; set; }
        //Un banquete pertenece unicamente a un cliente
        virtual public Cliente cliente { get; set; }

        //Un banquete puede tener muchos pagos asociados a él.
        virtual public ICollection<Pago> pagos { get; set; }
    }
}