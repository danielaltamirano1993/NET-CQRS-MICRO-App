using MediatR;

namespace Microservicio.Usuarios.API.Application.Commands
{
    public record CrearUsuarioCommand(
        string NombreUsuario, 
        string Correo
    ) : IRequest<int>;
}
