namespace Microservicio.Items.API.Domain
{
    public class HistorialAsignacion
    {
        public int HistorialId { get; set; }
        public int ItemId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public string EstadoAsignacion { get; set; } // Activa,
                                                     // Reasignada,
                                                     // Cancelada
    }
}
