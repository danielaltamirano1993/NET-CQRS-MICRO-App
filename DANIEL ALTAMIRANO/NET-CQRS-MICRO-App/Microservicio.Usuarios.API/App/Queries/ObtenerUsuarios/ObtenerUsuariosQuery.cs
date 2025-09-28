using MediatR;
using Microservicio.Usuarios.API.Domain;

namespace Microservicio.Usuarios.API.App.Queries.ObtenerUsuarios
{
    public record ObtenerUsuariosQuery() : IRequest<List<Usuario>>;
}
