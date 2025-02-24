﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace P01_2022AA650_2022MA653.Models
{
    public class Reservas
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int EspacioParqueoId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public int CantidadHoras { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuarios Usuario { get; set; }

        [ForeignKey("EspacioParqueoId")]
        public virtual EspaciosParqueo EspacioParqueo { get; set; }

    }
}
