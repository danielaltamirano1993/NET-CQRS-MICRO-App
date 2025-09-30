using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Microservicio.Items.API.Domain
{
    public class UsuarioReferencia
    {
        [Key]
        public int UsuarioId { get; set; }
        [Required]
        [MaxLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;
        public int ItemsPendientes { get; set; } = 0;
        public int ItemsCompletados { get; set; } = 0;
        public bool Activo { get; set; } = true;
                
        [JsonIgnore]
        public ICollection<ItemTrabajo>? ItemsAsignados { get; set; }
        [JsonIgnore]
        public ICollection<HistorialAsignacion>? Historiales { get; set; }
    }
}
