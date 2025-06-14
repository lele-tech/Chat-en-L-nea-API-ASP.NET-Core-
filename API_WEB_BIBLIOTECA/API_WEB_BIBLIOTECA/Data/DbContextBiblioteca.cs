//Referencia del ORM
using Microsoft.EntityFrameworkCore;
//Referencia para objeto modelo
using API_WEB_BIBLIOTECA.Models;

namespace API_WEB_BIBLIOTECA.Data
{
    public class DbContextBiblioteca : DbContext
    {
        //Constructor con parámetros
        public DbContextBiblioteca(DbContextOptions<DbContextBiblioteca> options) : base(options)
        {

        }

        //Propiedad para manejar los procesos CRUD el catalogo de libros
        public DbSet<Libro> Libros { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Message> Messages { get; set; }


    }
}
