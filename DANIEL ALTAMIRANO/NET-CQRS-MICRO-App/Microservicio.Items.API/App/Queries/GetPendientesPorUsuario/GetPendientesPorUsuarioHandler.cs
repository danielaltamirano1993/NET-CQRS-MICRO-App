using MediatR;
using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Queries.GetPendientesPorUsuario
{
    public class GetPendientesPorUsuarioHandler : IRequestHandler<GetPendientesPorUsuarioQuery, IEnumerable<ItemTrabajo>>
    {
        private readonly ItemDbContext _context;

        public GetPendientesPorUsuarioHandler(
            ItemDbContext context
        ) => _context = context;

        public async Task<IEnumerable<ItemTrabajo>> Handle(
            GetPendientesPorUsuarioQuery request,
            CancellationToken cancellationToken
        )
        {
            // 🛑 Lógica del SP refactorizada a LINQ:
            // 1. Filtrar por UsuarioAsignado y Estado
            var query = _context.ItemTrabajo
                .Where(i => i.UsuarioAsignado == request.UsuarioId &&
                            i.Estado != "Completado");

            // 2. Ordenar por FechaEntrega ASC y Relevancia DESC
            query = query
                .OrderBy(i => i.FechaEntrega)
                .ThenByDescending(i => i.Relevancia);

            // 3. Incluir las referencias (Opcional, si son necesarias)
            // Esto reemplaza las cargas manuales con .LoadAsync()
            query = query
                .Include(i => i.Usuario)
                .Include(i => i.Historiales);

            // 4. Ejecutar la consulta
            return await query.ToListAsync(cancellationToken);
        }
    }
}