using Microsoft.EntityFrameworkCore;
using System.Data;

namespace P01_2022AA650_2022MA653.Models
{
    public class ClaseContext: DbContext
    {
        public ClaseContext(DbContextOptions<ClaseContext> options) : base(options)
        {

        }
/*
        public DbSet<usuarios> Usuarios { get; set; }
        public DbSet<comentarios> comentarios { get; set; }
        public DbSet<roles> roles { get; set; }
        public DbSet<calificaciones> calificaciones { get; set; }
        public DbSet<publicaciones> publicaciones { get; set; }*/
    }
}
