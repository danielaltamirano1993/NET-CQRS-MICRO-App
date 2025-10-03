using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicio.Items.API.Domain
{
    public partial class ItemTrabajo
    {
        public ItemTrabajo()
        {
        }

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
        public byte Relevancia { get; set; } = 1;

        public int Orden { get; internal set; } = 0; // 0 por defecto

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public int? UsuarioAsignado { get; set; }
        public UsuarioReferencia? Usuario { get; set; }

        public ICollection<HistorialAsignacion> Historiales { get; set; } = new List<HistorialAsignacion>();
    }
}
