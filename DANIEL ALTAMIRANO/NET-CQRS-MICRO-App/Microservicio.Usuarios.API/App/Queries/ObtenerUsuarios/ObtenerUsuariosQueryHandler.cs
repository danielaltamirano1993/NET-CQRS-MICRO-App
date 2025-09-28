using MediatR;
using Microservicio.Usuarios.API.App.Queries.ObtenerUsuarios;
using Microservicio.Usuarios.API.Domain;
using Microservicio.Usuarios.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Usuarios.API.Application.Queries
{
    public class ObtenerUsuariosQueryHandler : IRequestHandler<ObtenerUsuariosQuery, List<Usuario>>
    {
        private readonly UsuarioDbContext _context;

        public ObtenerUsuariosQueryHandler(
            UsuarioDbContext context
        )
        {
            _context = context;
        }

        public async Task<List<Usuario>> Handle(
            ObtenerUsuariosQuery request, 
            CancellationToken cancellationToken
        )
        {
            return await _context.Usuarios
                .Include(u => u.Estadistica)
                .ToListAsync(cancellationToken);
        }
    }
}
