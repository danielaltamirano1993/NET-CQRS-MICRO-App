namespace Microservicio.Items.API.App.Dto
{
    public class UsuarioExternoDto
    {
        public int Id { get; set; } // El ID externo
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public int? LimiteItems { get; set; } // O int si no es nullable
    }
}
