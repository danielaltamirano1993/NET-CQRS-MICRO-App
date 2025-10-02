using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicio.Items.API.Domain
{
    public partial class HistorialAsignacion
    {
        protected HistorialAsignacion() { }

        [Key]
        public int HistorialId { get; set; }

        [Required]
        public int ItemTrabajoId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaAsignacion { get; set; }

        [MaxLength(255)]
        public string Comentarios { get; set; } = string.Empty;
    }
}