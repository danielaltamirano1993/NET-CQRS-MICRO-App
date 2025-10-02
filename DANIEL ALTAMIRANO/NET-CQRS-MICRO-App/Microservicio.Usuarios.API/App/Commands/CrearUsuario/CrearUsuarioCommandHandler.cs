using MediatR;
using Microservicio.Usuarios.API.App.Commands.CrearUsuario;
using Microservicio.Usuarios.API.Domain;
using Microservicio.Usuarios.API.Infrastructure;

namespace Microservicio.Usuarios.API.Application.Commands
{
    public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, int>
    {
        private readonly UsuarioDbContext _context;

        public CrearUsuarioCommandHandler(
            UsuarioDbContext context
        )
        {
            _context = context;
        }

        public async Task<int> Handle(
            CrearUsuarioCommand request, 
            CancellationToken cancellationToken
        )
        {
            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Correo        = request.Correo,
                Activo        = request.Activo,
                FechaRegistro = DateTime.UtcNow
            };

            usuario.Estadistica = new UsuarioEstadistica
            {
                ItemsPendientes  = 0,
                ItemsCompletados = 0
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            return usuario.UsuarioId;
        }
    }
}
