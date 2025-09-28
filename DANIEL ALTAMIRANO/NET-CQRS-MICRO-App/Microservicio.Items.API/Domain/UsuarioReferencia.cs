namespace Microservicio.Items.API.Domain
{
    public class UsuarioReferencia
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int ItemsPendientes { get; set; }
        public int ItemsAltamenteRelevantes { get; set; }
        public bool Activo { get; set; }
    }
}
