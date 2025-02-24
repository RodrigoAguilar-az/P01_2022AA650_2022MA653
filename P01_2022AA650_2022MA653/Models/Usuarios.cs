using System.ComponentModel.DataAnnotations;

namespace P01_2022AA650_2022MA653.Models
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public string Contraseña { get; set; }

        [Required]
        public string Rol { get; set; }

        public virtual ICollection<Sucursales> Sucursales { get; set; }
        public virtual ICollection<Reservas> Reservas { get; set; }
    }
}
