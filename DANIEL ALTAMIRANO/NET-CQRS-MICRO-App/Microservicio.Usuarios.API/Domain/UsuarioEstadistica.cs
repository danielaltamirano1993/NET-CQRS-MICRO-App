using Microservicio.Usuarios.API.Domain;

public class UsuarioEstadistica
{
    public int EstadisticaId { get; set; }
    public int UsuarioId { get; set; }
    public int ItemsPendientes { get; set; }
    public int ItemsCompletados { get; set; }
    public DateTime UltimaActualizacion { get; set; }

    public Usuario Usuario { get; set; }
}
