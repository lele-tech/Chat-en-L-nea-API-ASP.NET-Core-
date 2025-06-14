using Microsoft.AspNetCore.Mvc;
using AppWebBibliotecaConsumo.Data;

using AppWebBibliotecaConsumo.Models;
using Newtonsoft;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace AppWebBibliotecaConsumo.Controllers
{
    public class LibrosController : Controller
    {

        //Variable  Object que permite manejar la referencia la API web
        private LibrosApi librosAPI;


        //Variable para manejar las transacciones del  protocolo  HttpClient
        private HttpClient client;


        /// <summary>
        /// _Constructor con parámetros nos permite instanciar los recursos globales para el controller
        /// </summary>
        public LibrosController()
        {
            //Se instancia  el object
            librosAPI = new LibrosApi();

            //Se inicializa el object HttpClient
            client = librosAPI.Inicial();

        }

        public async Task<IActionResult> Index()
        {
            //Lista para almacenar los datos
            List<Libro> libros = new List<Libro>();

            //Se utiliza el método  listado  publicado en la API web
            HttpResponseMessage response = await client.GetAsync("/Libros/Listado");

            //se valida  si la respuesta es correcta
            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                libros = JsonConvert.DeserializeObject<List<Libro>>(resultado);
            }

            return View(libros);
        }


        /// <summary>
        /// Método para registrar  un libro utilizando la API Web
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Libro pLibro)
        {
            //El ISBN del libro se genera de forma automatica
            pLibro.ISBN = 0;


            //se asigna el token de autorización
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();


            //Se utiliza la API web  para almacenar los datos del libro
            var agregar = client.PostAsJsonAsync<Libro>("/Libros/Agregar", pLibro);

            await agregar;  //se espera que termine la transacción

            //Emmm vemos  el resultado
            var resultado = agregar.Result;

            if (resultado.IsSuccessStatusCode) //si todo fue correcto
            {
                //se ubica al usuario dentro del listado  libros
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Mensaje"] = "No se logró registrar el libro..";
                return View(pLibro);
            }

        }


        //Método encargado de realizar los procesos de editar los datos utilizando la API web
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Variable para almacenar los datos del libro
            var libro = new Libro();

            //Se utiliza la  API web para buscar el libro que deseamos editar los datos
            HttpResponseMessage response = await client.GetAsync($"/Libros/Buscar?isbn={id}");

            //Se valida si la respuesta fue correcta
            if (response.IsSuccessStatusCode)
            {
                //Se realiza la lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;

                //Se conviert el JSON en un  Object
                libro = JsonConvert.DeserializeObject<Libro>(resultado);
            }

            //Se envia al front.end los datos del  object
            return View(libro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Libro pLibro)
        {
            //Se asigna el token de autorización
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            //Se utiliza la API web su método  enviado los datos del libro a editar
            var modificar = client.PutAsJsonAsync<Libro>("/Libros/Editar", pLibro);
            await modificar; //Esperamos

            //se toma el resultado
            var resultado = modificar.Result;

            //se valida la respuesta
            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                //se almacena un mensaje de error
                TempData["Mensaje"] = "Datos incorrectos..";

                if (resultado.StatusCode.ToString().Equals("Unauthorized"))
                {
                    return RedirectToAction("Login", "Usuarios");
                }

                //se ubica al usuario dentro del formulario editar para que verifique los datos
                return View(pLibro);
            }


        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var libro = new Libro();

            //Se utiliza la  API web para buscar el libro que deseamos editar los datos
            HttpResponseMessage response = await client.GetAsync($"/Libros/Buscar?isbn={id}");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                libro = JsonConvert.DeserializeObject<Libro>(resultado);
            }

            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            //Se asigna el token de autorización
            client.DefaultRequestHeaders.Authorization = AutorizacionToken();

            //se utiliza la API web  su método eliminar se envia como parámetro del isbn del  libro a eliminar
            HttpResponseMessage response = await client.DeleteAsync($"/Libros/Eliminar?isbn={id}");

            if (response.StatusCode.ToString().Equals("Unauthorizad"))
            {
                //Se ubica dentro del catalogo de libros
                return RedirectToAction("Login", "Usuarios");
            }
            else
            {
                //Re ubica dentro del catalogo de libros
                return RedirectToAction("Index");
            }
        }

        private AuthenticationHeaderValue AutorizacionToken()
        {
            //se extrae el token almacenado dentro de la sesión
            var token = HttpContext.Session.GetString("token");

            //Variable para almacenar el token de autenticación
            AuthenticationHeaderValue authentication = null;

            if (token != null && token.Length != 0)
            {
                //Se almacena el token  otorgado por la API
                authentication = new AuthenticationHeaderValue("Bearer", token);
            }

            //se retorna la información
            return authentication;
        }

    }//cierre controller
} //cierre namespaces


