namespace Microservicio.Usuarios.API.Domain
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Relación 1:1 con UsuarioEstadistica
        public UsuarioEstadistica Estadistica { get; set; }
    }
}
