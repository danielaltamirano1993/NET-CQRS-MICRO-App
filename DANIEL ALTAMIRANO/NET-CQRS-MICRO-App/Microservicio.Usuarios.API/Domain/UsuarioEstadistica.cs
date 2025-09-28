using Microservicio.Usuarios.API.Domain;
using System.Text.Json.Serialization;

public class UsuarioEstadistica
{
    public int EstadisticaId { get; set; }
    public int UsuarioId { get; set; }
    public int ItemsPendientes { get; set; }
    public int ItemsCompletados { get; set; }
    public DateTime UltimaActualizacion { get; set; }

    [JsonIgnore]
    public Usuario Usuario { get; set; }
}
