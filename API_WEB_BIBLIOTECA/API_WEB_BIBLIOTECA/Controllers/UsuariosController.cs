using API_WEB_BIBLIOTECA.Data;
using API_WEB_BIBLIOTECA.Models;
using API_WEB_BIBLIOTECA.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using API_WEB_BIBLIOTECA.Models.Custom;

namespace API_WEB_BIBLIOTECA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : Controller
    {
        private readonly DbContextBiblioteca _context;

        private readonly IAutorizacionService _autorizacionService;

        public UsuariosController(DbContextBiblioteca context, IAutorizacionService autorizacionService)
        {
            //Se asignan las referencias
            _context = context;
            _autorizacionService = autorizacionService;
        }

        [HttpGet("Listado")]
        public List<Usuario> Listado()
        {
            List<Usuario> lista = null;


            return lista;
        }

        [HttpPost("Agregar")]
        public async Task<string> Agregar(Usuario temp)
        {
            string mensaje = "Debe ingresar la información del usuario";

            if (temp == null)
            {
                return mensaje;
            }
            else
            {
                try
                {
                    _context.Usuarios.Add(temp);

                    await _context.SaveChangesAsync();

                    mensaje = $"Usuario {temp.NombreCompleto} almacenado con exito..";
                }
                catch (Exception ex)
                {
                    mensaje = $"Error al agregar el usuario {temp.NombreCompleto} detalle {ex.InnerException}";
                }
                return mensaje;
            }
        }

        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(string email)
        {
            string mensaje = $"Usuario no eliminado {email} valor no existe..";

            Usuario temp = _context.Usuarios.FirstOrDefault(x => x.Email == email);

            if (temp != null)
            {
                _context.Usuarios.Remove(temp);
                await _context.SaveChangesAsync();
                mensaje = $"Usuario {temp.NombreCompleto} eliminado correctamente...";
            }

            return mensaje;
        }

        [HttpPut("Editar")]
        public async Task<string> Editar(Usuario temp)
        {
            var aux = _context.Usuarios.FirstOrDefault(x => x.Email == temp.Email);

            string mensaje = "";

            if (aux != null)
            {
                aux.NombreCompleto = temp.NombreCompleto;
                aux.Password = temp.Password;
                aux.FechaRegistro = temp.FechaRegistro;
                aux.Estado = temp.Estado;

                _context.Usuarios.Update(aux);

                await _context.SaveChangesAsync();

                mensaje = $"usuario {aux.Email} actualizado correctamente..";
            }
            else
            {
                mensaje = $"El usuario {temp.NombreCompleto} no existe ...";
            }
            return mensaje;
        }

        [HttpGet("Buscar")]
        public Usuario Buscar(string email)
        {
            Usuario temp = null;

            temp = _context.Usuarios.FirstOrDefault(h => h.Email == email);

            return temp == null ? new Usuario() { NombreCompleto = "No existe" } : temp;

        }

     
        [HttpPost]
        [Route("AutenticarPW")]
        public async Task<IActionResult> AutenticarPW(string email, string password)
        {
            //Se valida el email y pw deben estar correctos
            var temp = await _context.Usuarios.FirstOrDefaultAsync( x => x.Email.Equals(email) && x.Password.Equals(password));

            if (temp == null)
            {
                return Unauthorized(new AutorizacionResponse() { Token="", Msj="No autorizado",Resultado=false}); //No está autorizado
            }
            else
            {
                //Si hay datos se genera el token
                var autorizado = await _autorizacionService.DevolverToken(temp);

                if (autorizado == null)
                {
                    //No está autorizado
                    return Unauthorized();
                }
                else
                {
                    //Se devuelve el token
                    return Ok(autorizado);
                }
            }
        }



    }
}
