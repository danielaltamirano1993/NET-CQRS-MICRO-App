using MediatR;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Domain;
using System.Collections.Generic;

namespace Microservicio.Items.API.App.Queries.ConsultarTodosLosItems
{
    public record ConsultarTodosLosItemsQuery() : IRequest<IEnumerable<ItemTrabajoDto>>;
}