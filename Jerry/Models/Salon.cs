using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jerry.Models
{
    public class Salon
    {
        [Key]
        public int salonID { get; set; }

        [Required]
        [Display(Name ="Salón")]
        public string nombre { get; set; }

        [Required]
        [Display(Name ="Descripción")]
        public string detalles { get; set; }

        //Un salon puede tener una coleccion de reservaciones, es decir, puede tener varias reservaciones
        virtual public ICollection<SesionDeReservacion> sesionesReservadas { get; set; }
        
        /// <summary>
        /// Arroja aregglo objetos enumeradas en la clase estatica Salon.
        /// </summary>
        /// <returns>Arreglo de objectos dinamicos con Text y Value como atributos,
        /// ambos con el nombre.</returns>
        public static List<object> getSalonesItemArray()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var salones = db.salones.ToList();
            List<object> array = new List<object>();

            foreach(var salon in salones)
            {
                array.Add(new { Text = salon.nombre, Value = salon.salonID});
            }

            return array;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getSalonesSelectList(object selVal = null)
        {
            return new SelectList(Salon.getSalonesItemArray(), "Value", "Text", selVal);
        }
    }
}