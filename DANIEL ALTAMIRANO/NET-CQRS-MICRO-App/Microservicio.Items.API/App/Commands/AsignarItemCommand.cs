using MediatR;

namespace Microservicio.Items.API.Application.Commands
{
    public record AsignarItemCommand(
        int ItemId
    ) : IRequest<bool>;
}
