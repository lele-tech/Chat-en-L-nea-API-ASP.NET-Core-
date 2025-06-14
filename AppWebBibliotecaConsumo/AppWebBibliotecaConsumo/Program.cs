using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddSignalR(); // Agrega SignalR

// Configuración de la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(config =>
    {
        config.Cookie.Name = "UserLoginCookie";
        config.LoginPath = "/Usuarios/Login";
        config.Cookie.HttpOnly = true;
        config.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        config.AccessDeniedPath = "/Usuarios/AccessDenied";
        config.SlidingExpiration = true;
    });

// Configuración de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuración de controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline de solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirigir a HTTPS
app.UseStaticFiles(); // Servir archivos estáticos

app.UseRouting();

// Uso de sesión y autenticación
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Mapeo del Hub de SignalR
app.MapHub<ChatHub>("/chatHub");  // Asegúrate de que esta ruta esté correctamente configurada

// Configuración de las rutas de los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicación
app.Run();
