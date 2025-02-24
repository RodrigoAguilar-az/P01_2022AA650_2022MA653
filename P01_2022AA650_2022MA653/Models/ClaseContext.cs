using Microsoft.EntityFrameworkCore;
using System.Data;

namespace P01_2022AA650_2022MA653.Models
{
    public class ClaseContext : DbContext
    {
        public ClaseContext(DbContextOptions<ClaseContext> options) : base(options)
        {

        }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Sucursales> Sucursales { get; set; }
        public DbSet<EspaciosParqueo> EspaciosParqueos { get; set; }
        public DbSet<Reservas> Reservas { get; set; }
    }
}
