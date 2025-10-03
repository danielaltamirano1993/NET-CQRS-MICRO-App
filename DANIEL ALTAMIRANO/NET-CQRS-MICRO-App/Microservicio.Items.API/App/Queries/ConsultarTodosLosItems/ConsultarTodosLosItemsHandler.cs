using MediatR;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Queries.ConsultarTodosLosItems
{
    public class ConsultarTodosLosItemsQueryHandler
        : IRequestHandler<ConsultarTodosLosItemsQuery, IEnumerable<ItemTrabajoDto>>
    {
        private readonly ItemDbContext _context;

        public ConsultarTodosLosItemsQueryHandler(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemTrabajoDto>> Handle(
            ConsultarTodosLosItemsQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _context.ItemTrabajo
                .AsNoTracking()
                .Select(i => new ItemTrabajoDto
                {
                    ItemId = i.ItemId,
                    Titulo = i.Titulo,
                    Descripcion = i.Descripcion,
                    FechaCreacion = i.FechaCreacion,
                    FechaEntrega = i.FechaEntrega,
                    Relevancia = i.Relevancia,
                    Estado = i.Estado,
                    UsuarioAsignadoId = i.UsuarioAsignado ?? 0,
                    NombreUsuarioAsignado = i.Usuario != null ? i.Usuario.NombreUsuario : string.Empty
                })
                .ToListAsync(cancellationToken);
        }
    }
}
