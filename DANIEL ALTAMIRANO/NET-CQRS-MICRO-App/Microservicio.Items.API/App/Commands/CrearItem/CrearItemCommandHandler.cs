using MediatR;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Commands.CrearItem
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
            var item = ItemTrabajo.Crear(
                request.Titulo,
                request.Descripcion,
                request.FechaEntrega,
                request.Relevancia 
            );

            _context.ItemTrabajo.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            return item.ItemId;
        }
    }
}
