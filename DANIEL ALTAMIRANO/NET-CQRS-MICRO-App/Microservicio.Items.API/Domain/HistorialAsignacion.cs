using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Microservicio.Items.API.Domain
{
    public class HistorialAsignacion
    {
        [Key]
        public int HistorialId { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string EstadoAsignacion { get; set; } = "Activa"; // Activa, Reasignada, Cancelada

        //// Relaciones        
        //[JsonIgnore]
        //public ICollection<ItemTrabajo>? ItemsAsignados { get; set; }
        [JsonIgnore]
        public ItemTrabajo Item { get; set; }
        public UsuarioReferencia? Usuario { get; set; }
    }
}
