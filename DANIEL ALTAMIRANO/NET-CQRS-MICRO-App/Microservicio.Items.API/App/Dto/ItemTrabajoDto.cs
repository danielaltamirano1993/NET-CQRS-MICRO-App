namespace Microservicio.Items.API.App.Dto
{
    public class ItemTrabajoDto
    {
        public int ItemId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int Relevancia { get; set; }
        public string Estado { get; set; }

        public int UsuarioAsignadoId { get; set; }
        public string NombreUsuarioAsignado { get; set; }
    }

}
