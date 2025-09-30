using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicio.Items.API.Domain
{
    public class ItemTrabajo
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime FechaEntrega { get; set; }

        [Required]
        public byte Relevancia { get; set; } = 1; // 1 = Baja, 2 = Alta

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, En Proceso, Completado

        public int? UsuarioAsignado { get; set; }
        public UsuarioReferencia? Usuario { get; set; }

        [NotMapped]
        public ICollection<HistorialAsignacion> Historiales { get; set; }
    }
}
