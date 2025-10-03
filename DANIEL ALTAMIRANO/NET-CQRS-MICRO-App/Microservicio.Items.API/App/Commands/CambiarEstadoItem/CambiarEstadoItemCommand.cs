using MediatR;

namespace Microservicio.Items.API.App.Commands.CambiarEstadoItem
{
    public record CambiarEstadoItemCommand(
        string NuevoEstado 
    ) : IRequest<bool>
    {
        public int ItemId { get; init; }
    }
}
