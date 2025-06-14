using API_WEB_BIBLIOTECA.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Se agrega la inyección de dependencia al ORM Entityframework core que implementa
builder.Services.AddScoped<API_WEB_BIBLIOTECA.Data.DbContextBiblioteca>();

//Se agrega el servicio del ORM con DbContext además su string de conexion
builder.Services.AddDbContext<API_WEB_BIBLIOTECA.Data.DbContextBiblioteca>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("StringConexion")));

//Se debe configurar el servicio de JWT
//Se agrega la interface y el objeto que la implementa
builder.Services.AddScoped<IAutorizacionService, AutorizacionServices>();

//Se toma la llave a utilizar para generar el Token
var key = builder.Configuration.GetValue<string>("JwtSettings:Key");
var keyBytes = Encoding.ASCII.GetBytes(key);

//Se procede con la configuración para el esquema de autenticación
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false; //No requiere metadata
    config.SaveToken = true; //Se almacena el token
        config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true, //Valida la key para inicio sesión
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes), //Se asigna el valor para la key
            ValidateIssuer = false, //No se valida el emisor
            ValidateAudience = false, //No se valida la audiencia
            ValidateLifetime = true, //Se valida el tiempo de vida al token
            ClockSkew = TimeSpan.Zero //No debe existir diferencia desviación para el tiempo del reloj
        };
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Configuración de autenticación
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
