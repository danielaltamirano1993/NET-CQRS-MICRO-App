namespace Microservicio.Items.API.App.Dto
{
    public class CrearItemRequest
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int Relevancia { get; set; }
    }
}
