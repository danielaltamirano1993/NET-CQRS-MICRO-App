using System.Text.Json.Serialization;

namespace Microservicio.Items.API.App.Services.Dtos
{
    public class UsuarioExternoDto
    {
        [JsonPropertyName("usuarioId")]
        public int UsuarioId { get; set; }

        [JsonPropertyName("nombreUsuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }
    }
}
