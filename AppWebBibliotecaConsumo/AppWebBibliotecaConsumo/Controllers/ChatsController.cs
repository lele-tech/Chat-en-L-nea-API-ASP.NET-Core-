using Microsoft.AspNetCore.Mvc;
using AppWebBibliotecaConsumo.Data;
using AppWebBibliotecaConsumo.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AppWebBibliotecaConsumo.Controllers
{
    public class ChatsController : Controller
    {
        private LibrosApi librosAPI;
        private HttpClient client;

        public ChatsController()
        {
            librosAPI = new LibrosApi();
            client = librosAPI.Inicial();
        }

        // Acción para crear un mensaje
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.UserName) || string.IsNullOrEmpty(message.Mensaje) || string.IsNullOrEmpty(message.GroupName))
            {
                return Json(new { success = false, message = "Los datos no son válidos." });
            }

            var agregar = await client.PostAsJsonAsync("/Message/Agregar", message);

            if (agregar.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            else
            {
                TempData["Mensaje"] = "No se logró registrar el mensaje.";
                return Json(new { success = false, message = "Hubo un error al registrar el mensaje." });
            }
        }

        // Acción para manejar la vista de Chat y mostrar todos los mensajes
        public async Task<IActionResult> Chat()
        {
            List<Message> messages = new List<Message>();

            try
            {
                HttpResponseMessage response = await client.GetAsync("/Message/Listado");

                if (response.IsSuccessStatusCode)
                {
                    string resultado = await response.Content.ReadAsStringAsync();
                    messages = JsonConvert.DeserializeObject<List<Message>>(resultado);
                }
                else
                {
                    TempData["Mensaje"] = "No se pudieron obtener los mensajes.";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error al obtener los mensajes: {ex.Message}";
            }

            // Pasar los mensajes al frontend como JSON
            ViewData["Messages"] = JsonConvert.SerializeObject(messages);

            // Retornar la vista (la vista cargará los mensajes usando JavaScript)
            return View();
        }

        // Acción para mostrar un mensaje específico a editar
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var message = new Message();

            HttpResponseMessage response = await client.GetAsync($"/Message/Buscar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();
                message = JsonConvert.DeserializeObject<Message>(resultado);
            }

            return View(message);
        }

        // Acción para procesar la edición de un mensaje
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Message message)
        {
            var modificar = await client.PutAsJsonAsync("/Message/Editar", message);

            if (modificar.IsSuccessStatusCode)
            {
                return RedirectToAction("Chat");
            }
            else
            {
                TempData["Mensaje"] = "Error al modificar el mensaje.";
                return View(message);
            }
        }

        // Acción para mostrar un mensaje específico a eliminar
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var message = new Message();

            HttpResponseMessage response = await client.GetAsync($"/Message/Buscar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();
                message = JsonConvert.DeserializeObject<Message>(resultado);
            }

            return View(message);
        }

        // Acción para eliminar un mensaje
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"/Message/Eliminar?id={id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Chat");
            }
            else
            {
                TempData["Mensaje"] = "Error al eliminar el mensaje.";
                return RedirectToAction("Chat");
            }
        }

        // Acción para obtener los mensajes de un grupo
        public async Task<IActionResult> LoadMessages(string grupo)
        {
            List<Message> messages = new List<Message>();

            try
            {
                // Solo cargar los mensajes al principio, cuando entras al grupo
                HttpResponseMessage response = await client.GetAsync("/Message/Listado");

                if (response.IsSuccessStatusCode)
                {
                    string resultado = await response.Content.ReadAsStringAsync();
                    messages = JsonConvert.DeserializeObject<List<Message>>(resultado);

                    // Filtrar los mensajes según el grupo especificado
                    if (!string.IsNullOrEmpty(grupo))
                    {
                        switch (grupo)
                        {
                            case "Grupo1":
                                // Filtrar por GroupName = "Grupo1"
                                messages = messages.Where(m => m.GroupName == "Grupo1").ToList();
                                break;
                            case "Grupo2":
                                // Filtrar por GroupName = "Grupo2"
                                messages = messages.Where(m => m.GroupName == "Grupo2").ToList();
                                break;
                            case "Grupo3":
                                // Filtrar por GroupName = "Grupo3"
                                messages = messages.Where(m => m.GroupName == "Grupo3").ToList();
                                break;
                            default:
                                // Si no es ninguno de los grupos definidos, no filtrar
                                break;
                        }
                    }
                }
                else
                {
                    return BadRequest("No se pudieron obtener los mensajes.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener los mensajes: {ex.Message}");
            }

            return Json(messages); // Devolver solo los mensajes filtrados como JSON
        }
    }
}
