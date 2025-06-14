using System.ComponentModel.DataAnnotations;

namespace API_WEB_BIBLIOTECA.Models
{
    public class Libro
    {
        [Key]
        [Required(ErrorMessage ="Se debe ingresar el identificar del libro")]
        public int ISBN { get; set; }

        [Required(ErrorMessage ="No se permite el titulo en blanco")]
        [StringLength(150)]
        [MinLength(10)]
        [Display(Name ="Titulo del libro")]
        public string Titulo { get; set; }

        [Required(ErrorMessage ="Debe indicar el nombre de la editorial")]
        [MinLength (10)]
        [MaxLength(150)]
        public string Editorial { get; set; }

        public int Annio { get; set; }

        [Display(Name ="Precio de compra")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0}")]
        public decimal PrecioCompra { get; set;
        }

        [Required(ErrorMessage = "Debe seleccionar el estado del libro")]
        public char Estado { get; set; }

        [Required(ErrorMessage ="Seleccione la fecha de registro")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString ="{0:d}")]
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Propiedad para almacenar la ubicación del archivo
        /// </summary>
        public string Foto { get; set; }

    }
}
