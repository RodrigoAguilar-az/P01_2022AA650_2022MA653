using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace P01_2022AA650_2022MA653.Models
{
    public class EspaciosParqueo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SucursalId { get; set; }

        [Required]
        public int Numero { get; set; }

        [Required]
        [StringLength(255)]
        public string Ubicacion { get; set; }

        [Required]
        public decimal CostoPorHora { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        // Propiedades de navegación
        [ForeignKey("SucursalId")]
        public virtual Sucursales Sucursal { get; set; }

        public virtual ICollection<Reservas> Reservas { get; set; }
    }
}
