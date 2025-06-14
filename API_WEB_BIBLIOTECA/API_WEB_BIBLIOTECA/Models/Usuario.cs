using System.ComponentModel.DataAnnotations;

namespace API_WEB_BIBLIOTECA.Models
{
    public class Usuario
    {
        [Key]
        [Required(ErrorMessage = "Debe ingresar el email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "No se permite el nombre en blanco")]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "No se permite el password en blanco")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Confirmar(string pw)
        {
            bool confirmado = false;
            if (Password != null)
            {
                if (Password.Equals(pw))
                {
                    confirmado = true;
                }
            }
            return confirmado;
        }


        [Required(ErrorMessage = "Debe ingresar la Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "Debe ingresar su Estado")]
        public char Estado {  get; set; }
    }
}
