using Microservicio.Items.API.Domain;
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
        public ICollection<ItemTrabajo>? ItemsAsignados { get; set; } = new List<ItemTrabajo>();
        [JsonIgnore]
        public ICollection<HistorialAsignacion>? Historiales { get; set; } = new List<HistorialAsignacion>();
    }
}



