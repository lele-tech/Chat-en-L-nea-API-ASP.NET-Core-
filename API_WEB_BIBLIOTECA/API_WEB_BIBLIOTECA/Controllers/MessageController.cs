using API_WEB_BIBLIOTECA.Data;
using API_WEB_BIBLIOTECA.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_WEB_BIBLIOTECA.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : Controller
    {
        private readonly DbContextBiblioteca _context = null;

        public MessageController(DbContextBiblioteca pContext)
        {
            _context = pContext;
        }

        // Método encargado de mostrar la información de todos los mensajes
        [HttpGet("Listado")]
        public List<Message> Listado()
        {
            return _context.Messages.ToList();
        }

        // Método encargado de almacenar un mensaje
        [HttpPost("Agregar")]
        public async Task<string> Agregar(Message temp)
        {
            string mensaje = "Debe ingresar la información del mensaje";

            if (temp == null)
            {
                return mensaje;
            }
            else
            {
                try
                {
                    _context.Messages.Add(temp);
                    await _context.SaveChangesAsync();
                    mensaje = $"Mensaje de {temp.UserName} almacenado con éxito.";
                }
                catch (Exception ex)
                {
                    mensaje = $"Error al agregar el mensaje, detalle: {ex.InnerException}";
                }
                return mensaje;
            }
        }

        // Método encargado de eliminar un mensaje por medio de su ID
        [HttpDelete("Eliminar")]
        public async Task<string> Eliminar(int id)
        {
            string mensaje = $"Mensaje no eliminado. ID {id} no existe.";

            Message temp = _context.Messages.FirstOrDefault(x => x.Id == id);

            if (temp != null)
            {
                _context.Messages.Remove(temp);
                await _context.SaveChangesAsync();
                mensaje = $"Mensaje de {temp.UserName} eliminado correctamente.";
            }

            return mensaje;
        }

        // Método encargado de editar un mensaje
        [HttpPut("Editar")]
        public async Task<string> Editar(Message temp)
        {
            var aux = _context.Messages.FirstOrDefault(x => x.Id == temp.Id);

            string mensaje = "";

            if (aux != null)
            {
                aux.GroupName = temp.GroupName;
                aux.UserName = temp.UserName;
                aux.Mensaje = temp.Mensaje;
                aux.TimeSent = temp.TimeSent;

                _context.Messages.Update(aux);
                await _context.SaveChangesAsync();

                mensaje = $"Mensaje {aux.Id} actualizado correctamente.";
            }
            else
            {
                mensaje = $"El mensaje con ID {temp.Id} no existe.";
            }
            return mensaje;
        }

        // Método encargado de consultar un mensaje por medio de su ID
        [HttpGet("Buscar")]
        public Message Buscar(int id)
        {
            Message temp = _context.Messages.FirstOrDefault(h => h.Id == id);
            return temp == null ? new Message() { Mensaje = "No existe" } : temp;
        }


        [HttpGet("Chat/{groupName}")]
        public IActionResult GetMessages(string groupName)
        {
            var messages = _context.Messages
                .Where(m => m.GroupName == groupName)
                .OrderBy(m => m.TimeSent)
                .ToList();

            if (messages == null || !messages.Any())
            {
                return NotFound(); // Devuelve un 404 si no hay mensajes
            }

            return Ok(messages); // Devuelve los mensajes en formato JSON
        }
    }
}
