using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.Models
{
    public class Cliente
    {
        [Key]
        public int clienteID { get; set; }

        [Required]
        [Display (Name ="Nombre")]
        public string nombre { get; set; }

        [Required]
        [Display(Name ="Apellido Paterno")]
        public string apellidoP { get; set; }

        [Required]
        [Display(Name ="Apellido Materno")]
        public string apellidoM { get; set; }

        [Required]
        [Display(Name ="Email")]
        public string email { get; set; }

        [Required]
        [Display(Name ="Telefono")]
        public int telefono { get; set; }

        //Un cliente tiene una coleccion de reservaciones, es decir, puede tener varias reservaciones
        virtual public ICollection<Reservacion> reservaciones { get; set; }
    }
}