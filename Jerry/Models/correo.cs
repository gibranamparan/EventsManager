using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jerry.Models
{
    public class Correo
    {
        [Key]
        public int correoID { get; set; }
        
        public string To { get; set; }

        [Display(Name ="Asunto")]
        public string Subject { get; set; }
        [Display(Name = "Contenido")]
        public string Body { get; set; }
        [Display(Name = "Correo administrador")]
        public string correoAdmin { get; set; }
        [Display(Name = "Contraseña")]
        public string contrasena { get; set; }
        [Display(Name = "smtpHost")]
        public string smtpHost { get; set; }
        [Display(Name = "Puerto")]
        public int puertoCorreo { get; set; }

    }
}