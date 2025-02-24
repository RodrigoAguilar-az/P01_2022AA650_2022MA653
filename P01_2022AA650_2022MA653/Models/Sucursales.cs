using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace P01_2022AA650_2022MA653.Models
{
    public class Sucursales
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Direccion { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int NumeroEspaciosDisponibles { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuario { get; set; }

        public virtual ICollection<EspaciosParqueo> EspaciosParqueo { get; set; }
    }
}
