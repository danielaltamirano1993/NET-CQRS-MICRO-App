using MediatR;
using Microservicio.Items.API.Domain;
using System.Collections.Generic;

namespace Microservicio.Items.API.App.Queries.GetPendientesPorUsuario
{
    public record GetPendientesPorUsuarioQuery(
        int UsuarioId
    ) : IRequest<IEnumerable<ItemTrabajo>>;
}
