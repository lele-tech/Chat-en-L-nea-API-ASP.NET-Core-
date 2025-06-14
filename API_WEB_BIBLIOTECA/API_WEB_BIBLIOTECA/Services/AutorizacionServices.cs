//Referencia para los objetos modelos
using API_WEB_BIBLIOTECA.Models;

//Referencia para utilizar el AutorizacionResponse
using API_WEB_BIBLIOTECA.Models.Custom;

//Referencia para utilizar el DbContext
using API_WEB_BIBLIOTECA.Data;

//Referencia para utilizar las librerias del ORM
using Microsoft.EntityFrameworkCore;

//Librerias para implementar JWT
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace API_WEB_BIBLIOTECA.Services
{
    public class AutorizacionServices : IAutorizacionService
    {
        //Variable para utilizar el archivo appsettings.json
        private readonly IConfiguration _configuration;

        //Variable para utilizar el DbContext con todos los métodos del ORM
        private readonly DbContextBiblioteca _context;

        public AutorizacionServices(IConfiguration configuration, DbContextBiblioteca dbContext)
        {
            _configuration = configuration;
            _context = dbContext;
        }

        public async Task<AutorizacionResponse> DevolverToken(Usuario autorizacion)
        {
            //Se identifica al usuario que está solicitando la autorización
            //Se valida su email y password
            var temp = await _context.Usuarios.FirstOrDefaultAsync( u => u.Email.Equals(autorizacion.Email) && u.Password.Equals(autorizacion.Password) );

            if(temp == null)
            {
                return await Task.FromResult<AutorizacionResponse>(null);
            }
            else
            {
                //Se genera el token
                string tokenCreado = GenerarToken(autorizacion.Email.ToString());

                //Se retorna la autorización con su token
                return new AutorizacionResponse() { Token = tokenCreado, Resultado = true, Msj="Ok" };
            }
        }

        private string GenerarToken(string IdUsuario)
        {
            //Se realiza la lectura de la key almacenada dentro del archivo de configuración JSON
            var key = _configuration.GetValue<string>("JwtSettings:key");

            //Se convierte la key en un vector de bytes
            var KeyBytes = Encoding.ASCII.GetBytes(key);

            //Se declara la identidad que realiza el reclamo para la solicitud de autorización
            var claims = new ClaimsIdentity();

            //Se asigna su identificador
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, IdUsuario));

            //Se instancian las credenciales del token
            var credencialesToken = new SigningCredentials( 
                new SymmetricSecurityKey(KeyBytes), //La llave que usa el algoritmo de cifrado 
                SecurityAlgorithms.HmacSha256Signature); //Algoritmo de cifrado

            //Se instancia el descriptor para token
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = claims, //Se asigna la identidad
                Expires = DateTime.UtcNow.AddMinutes(3), //Se agrega el tiempo de vida 3 minutos para el token
                SigningCredentials = credencialesToken //Se asignan las credenciales
            };

            //Se instancia el tokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Se crea el token
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            //Se escribe el token
            var tokenCreado = tokenHandler.WriteToken(tokenConfig);

            //Se retorna el token
            return tokenCreado;
        }

    }
}
