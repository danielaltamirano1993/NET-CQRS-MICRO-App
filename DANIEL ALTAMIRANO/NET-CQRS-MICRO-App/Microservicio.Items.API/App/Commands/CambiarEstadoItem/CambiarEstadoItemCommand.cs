using MediatR;

namespace Microservicio.Items.API.App.Commands.CambiarEstadoItem
{
    public record CambiarEstadoItemCommand(
        int ItemId,
        string NuevoEstado
    ) : IRequest<bool>;
}