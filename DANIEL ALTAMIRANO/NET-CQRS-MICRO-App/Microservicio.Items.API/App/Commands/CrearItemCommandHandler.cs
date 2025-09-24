using MediatR;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;

namespace Microservicio.Items.API.Application.Commands
{
    public class CrearItemCommandHandler : IRequestHandler<CrearItemCommand, int>
    {
        private readonly ItemDbContext _context;

        public CrearItemCommandHandler(
            ItemDbContext context
        ) => _context = context;

        public async Task<int> Handle(
            CrearItemCommand request, 
            CancellationToken cancellationToken
        ) 
        {
            var item = new ItemTrabajo
            {
                Titulo        = request.Titulo,
                Descripcion   = request.Descripcion,
                FechaCreacion = DateTime.UtcNow,
                FechaEntrega  = request.FechaEntrega,
                Relevancia    = request.Relevancia,
                Estado        = "Pendiente"
            };

            _context.ItemTrabajo.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return item.ItemId;
        }
    }
}
