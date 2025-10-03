using System.Text.Json.Serialization;

namespace Microservicio.Items.API.App.Dto
{
    public class UsuarioSincronizacionManualDto
    {
        // Mapea "usuarioId"
        [JsonPropertyName("usuarioId")]
        public int UsuarioId { get; set; }

        // Mapea "nombreUsuario"
        [JsonPropertyName("nombreUsuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        // Mapea "correo"
        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        // Mapea "activo"
        [JsonPropertyName("activo")]
        public bool Activo { get; set; }
    }
}
