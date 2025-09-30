using MediatR;

namespace Microservicio.Items.API.App.Commands.AsignarItem
{
    public record AsignarItemCommand(
        int ItemId
    ) : IRequest<int>; 
}