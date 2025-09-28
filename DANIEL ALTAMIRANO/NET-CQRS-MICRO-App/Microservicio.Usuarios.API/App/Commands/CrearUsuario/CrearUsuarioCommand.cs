using MediatR;

namespace Microservicio.Usuarios.API.App.Commands.CrearUsuario
{
    public record CrearUsuarioCommand(
        string NombreUsuario, 
        string Correo
    ) : IRequest<int>;
}
