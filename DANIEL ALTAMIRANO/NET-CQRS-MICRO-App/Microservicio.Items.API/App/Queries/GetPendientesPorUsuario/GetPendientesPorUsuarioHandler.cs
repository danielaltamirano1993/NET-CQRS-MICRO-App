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

            var items = await _context.ItemTrabajo
                .FromSqlRaw("EXEC dbo.sp_ListarPendientesPorUsuario " +
                            "@UsuarioId",
                            param
                           )
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
