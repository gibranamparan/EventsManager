using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Collections;

namespace Jerry.Models
{
    public class Banquete: Evento
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name ="Domicilio del Lugar")]
        public string lugar { get; set; }

        public Banquete()
        {
            this.TipoContrato = TipoDeContrato.SERVICIO;
        }

        /// <summary>
        /// Construye una lista de seleccion de contratos que aplican para esta clase de Evento.
        /// </summary>
        /// <param name="selVal">Optional, valor seleccionado por defecto.</param>
        /// <returns>Select list listo para ser usado en la vista rellenado</returns>
        public static SelectList getTipoContratoSelectList(string selVal = Reservacion.TiposContratoNombres.SERVICIO)
        {
            return new SelectList(Banquete.getTipoContratoItemArray(), "Value", "Text", selVal);
        }

        /// <summary>
        /// Arroja una lista de los tipos de contrato que aplican para esta clase de evento Banquetes.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable getTipoContratoItemArray()
        {
            List<object> array = new List<object>();
            array.Add(new { Text = Evento.getNombreContrato(TipoDeContrato.SERVICIO), Value = TipoDeContrato.SERVICIO });
            return array;
        }
    }
}