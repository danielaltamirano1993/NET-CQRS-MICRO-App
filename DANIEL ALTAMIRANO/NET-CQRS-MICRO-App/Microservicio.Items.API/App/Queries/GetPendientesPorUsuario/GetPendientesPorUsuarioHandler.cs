using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;

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
            var param = new SqlParameter(
                "@UsuarioId",
                request.UsuarioId
            );

            var results = await _context.ItemTrabajoSqlResults 
                .FromSqlRaw("EXEC dbo.sp_ListarPendientesPorUsuario " +
                            "@UsuarioId", 
                            param
                )
                .ToListAsync(cancellationToken);

            var items = results.Select(r => new ItemTrabajo
            {
                ItemId = r.ItemId,
                Titulo = r.Titulo,
                Descripcion = r.Descripcion,
                FechaCreacion = r.FechaCreacion,
                FechaEntrega = r.FechaEntrega,
                Relevancia = r.Relevancia,
                Estado = r.Estado,
                UsuarioAsignado = r.UsuarioAsignado,
                Historiales = new List<HistorialAsignacion>(), 
            }).ToList();

            foreach (var item in items)
            {
                _context.Attach(item);
                await _context.Entry(item)
                    .Reference(i => i.Usuario)
                    .LoadAsync(cancellationToken);

                await _context.Entry(item)
                    .Collection(i => i.Historiales)
                    .LoadAsync(cancellationToken);
            }
            return items;
        }
    }
}
