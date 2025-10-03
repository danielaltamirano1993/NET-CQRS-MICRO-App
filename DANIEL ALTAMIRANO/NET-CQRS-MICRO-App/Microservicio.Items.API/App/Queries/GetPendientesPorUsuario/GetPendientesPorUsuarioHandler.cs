using MediatR;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Queries.GetPendientesPorUsuario
{
    public class GetPendientesPorUsuarioQueryHandler
        : IRequestHandler<GetPendientesPorUsuarioQuery, IReadOnlyList<ItemTrabajoSqlResult>>
    {
        private readonly ItemDbContext _context;

        public GetPendientesPorUsuarioQueryHandler(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ItemTrabajoSqlResult>> Handle(
            GetPendientesPorUsuarioQuery request,
            CancellationToken cancellationToken
        )
        {   
            var itemsPendientes = await _context.ItemTrabajo
                .AsNoTracking() // Lectura
                .Where(i => i.UsuarioAsignado == request.UsuarioId &&
                            i.Estado != "Completado")
                                
                .OrderByDescending(i => i.Relevancia)
                .ThenBy(i => i.FechaEntrega)
                .Select(i => new ItemTrabajoSqlResult
                {
                    ItemId = i.ItemId,
                    Titulo = i.Titulo,
                    Descripcion = i.Descripcion,
                    FechaEntrega = i.FechaEntrega,
                    Relevancia = i.Relevancia,
                    Estado = i.Estado,
                    UsuarioAsignado = i.UsuarioAsignado.Value 
                })
                .ToListAsync(cancellationToken);

            return itemsPendientes;
        }
    }
}