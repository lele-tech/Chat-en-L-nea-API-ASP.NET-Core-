using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios
builder.Services.AddSignalR(); // Agrega SignalR

// Configuraci�n de la autenticaci�n con cookies
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

// Configuraci�n de sesi�n
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuraci�n de controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuraci�n del pipeline de solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirigir a HTTPS
app.UseStaticFiles(); // Servir archivos est�ticos

app.UseRouting();

// Uso de sesi�n y autenticaci�n
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Mapeo del Hub de SignalR
app.MapHub<ChatHub>("/chatHub");  // Aseg�rate de que esta ruta est� correctamente configurada

// Configuraci�n de las rutas de los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicaci�n
app.Run();
