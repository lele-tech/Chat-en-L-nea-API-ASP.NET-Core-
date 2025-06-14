using API_WEB_BIBLIOTECA.Data;
using API_WEB_BIBLIOTECA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_WEB_BIBLIOTECA.Controllers
{
    //Ojo etiquetas importantes para que nuestro controlador implemente los métodos API
    [ApiController]
    [Route("[controller]")]
    public class LibrosController : Controller
    {
        //Variable para utilizar la referencia del ORM Entity Framework core
        private readonly DbContextBiblioteca _context = null;

        /// <summary>
        /// Constructor con parámetros permite recibir la referencia del ORM
        /// </summary>
        /// <param name="pContext"></param>
        public LibrosController(DbContextBiblioteca pContext)
        {
            //Se asigna la referencia del ORM
            _context = pContext;
        }

        //Método encargado de mostrar la información de todos los libros
        [HttpGet("Listado")]
        public List<Libro> Listado()
        {
            //Variable tipo lista para almacenar la información de libros
            List<Libro> lista = null;

            //Se utiliza el ORM para tomar la información de la db
            lista = _context.Libros.ToList();

            //Se retorna la lista
            return lista;
        }

        //Método encargado de almacenar un libro
        [Authorize]
        [HttpPost("Agregar")]
        public async Task<string> Agregar(Libro temp)
        {
            string mensaje = "Debe ingresar la información del libro";

            //Se valida no hay datos
            if (temp == null)
            {
                return mensaje;
            }
            else
            {
                //Se intenta almacenar el libro en la db
                try
                {
                    //Se agrega el libro
                    _context.Libros.Add(temp);

                    //Se aplican los cambios
                    await _context.SaveChangesAsync();

                    //Se construye un mensaje de exito para el usuario
                    mensaje = $"Libro {temp.Titulo} almacenado con exito..";
                }
                catch (Exception ex)
                {
                    //En caso de un error se toma la información del exeptio para mostrar al usuario
                    mensaje = $"Error al agregar el libro {temp.Titulo} detalle {ex.InnerException}";
                }
                return mensaje;
            }
        }

        //Método encargado de eliminar un libro por medio de su isbn
        [Authorize]
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int isbn)
        {
            string mensaje = $"Libro no eliminado {isbn} valor no existe..";

            Libro temp = _context.Libros.FirstOrDefault(x => x.ISBN == isbn);

            if (temp != null)
            {
                _context.Libros.Remove(temp);
                await _context.SaveChangesAsync();
                mensaje = $"Libro {temp.Titulo} eliminado correctamente...";
            }

            return mensaje;
        }

        [Authorize]
        [HttpPut("Editar")]
        public async Task<string> Editar(Libro temp)
        {
            //Verificar si existe el libro dentro de la tabla
            var aux = _context.Libros.FirstOrDefault( x => x.ISBN == temp.ISBN);

            string mensaje = "";

            //Se pregunta si hay datos
            if (aux != null)
            {
                //Se actualizan los datos
                aux.Titulo = temp.Titulo;
                aux.Editorial = temp.Editorial;
                aux.PrecioCompra = temp.PrecioCompra;
                aux.Annio = temp.Annio;
                aux.FechaRegistro = temp.FechaRegistro;
                aux.Foto = temp.Foto;
                aux.Estado = temp.Estado;

                //Se actualiza la tabla
                _context.Libros.Update(aux);

                //Se aplican los cambios
                await _context.SaveChangesAsync();

                mensaje = $"Libro {aux.ISBN} actualizado correctamente..";
            }
            else
            {
                //Mensaje que no existe
                mensaje = $"El libro {temp.Titulo} no existe ...";
            }
            return mensaje;
        }

        //Método encargado de consultar un libro por medio de su ISBN
        [HttpGet("Buscar")]
        public Libro Buscar(int isbn)
        {
            //Variable para almacenar la información del libro
            Libro temp = null;

            //Se busca el libro de la tabla pro medio del ISBN
            temp = _context.Libros.FirstOrDefault( h => h.ISBN == isbn );

            //Se retorna la información
            return temp == null ? new Libro() { Titulo= "No existe"} : temp;

        }


    }
}
