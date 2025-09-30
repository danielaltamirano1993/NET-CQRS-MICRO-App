namespace Microservicio.Items.API.App.Dto
{
    public class ItemTrabajoSqlResult
    {
        public int ItemId { get; set; }
        public string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public byte Relevancia { get; set; }
        public string Estado { get; set; }
        public int? UsuarioAsignado { get; set; }
    }
}
