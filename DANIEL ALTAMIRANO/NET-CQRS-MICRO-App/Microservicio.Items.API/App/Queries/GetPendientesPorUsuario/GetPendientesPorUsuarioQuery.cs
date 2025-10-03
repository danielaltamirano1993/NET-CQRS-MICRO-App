using MediatR;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Microservicio.Items.API.App.Queries.GetPendientesPorUsuario
{
    public record GetPendientesPorUsuarioQuery(
        [FromRoute] int UsuarioId 
    ) : IRequest<IReadOnlyList<ItemTrabajoSqlResult>>;
}
