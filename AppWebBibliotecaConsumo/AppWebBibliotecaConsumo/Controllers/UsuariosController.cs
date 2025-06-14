using Microsoft.AspNetCore.Mvc;
using AppWebBibliotecaConsumo.Models;
using AppWebBibliotecaConsumo.Data;
using Newtonsoft.Json;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using API_WEB_BIBLIOTECA.Models.Custom;


namespace AppWebBibliotecaConsumo.Controllers
{
    public class UsuariosController : Controller
    {
        //Variable para utilizar la API
        private LibrosApi librosAPI;

        //Variable para utilizar los verbos del protocolo Http
        private HttpClient httpClient;

        /// <summary>
        /// Constructor por omision del controlador
        /// </summary>
        public UsuariosController()
        {
            //se instancia la  API
            librosAPI = new LibrosApi();

            //se inicializa el protocolo  httpClient
            httpClient = librosAPI.Inicial();
        }
        public IActionResult Index()
        {
            return View();
        }

        //Método encargado de realizar la autenticación
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] Usuario usuario)
        {
            //Variable para almacenar el token
            AutorizacionResponse autorizacion = null;

            if (usuario == null)
            {
                TempData["MensajeLogin"] = "Usuario o contraseña incorrectos..";
                return View(usuario);
            }
            //Se utiliza el método en la API para que nos genere el token
            HttpResponseMessage response = await httpClient.PostAsync($"/Usuarios/AutenticarPW?email={usuario.Email}&password={usuario.Password}", null);
            if (response.IsSuccessStatusCode)
            {
                //Se realiza lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;

                //Se convierten los datos JSON a un objeto con su token
                autorizacion = JsonConvert.DeserializeObject<AutorizacionResponse>(resultado);
            }

            //Se validan los datos de autorización
            if (autorizacion != null && autorizacion.Resultado == true)
            {

                //Se instancia la identidad a iniciar sesión
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                //Se rellenan los datos
                identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Email));

                //Se crea la identidad principal
                var principal = new ClaimsPrincipal(identity);

                //Se incia sesión
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //Se almacena el token otorgado
                HttpContext.Session.SetString("token", autorizacion.Token);

                //Se redirecciona al formulario principal
                return RedirectToAction("Index", "Home");

            }
            else
            {
                return View(usuario);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CrearCuenta()
        {
            return View();
        }


    }//cierre controller
} //cierre namespaces
