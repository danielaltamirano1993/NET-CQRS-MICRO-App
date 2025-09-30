using MediatR;

namespace Microservicio.Items.API.App.Commands.CompletarItemTrabajo
{
    public record CompletarItemTrabajoCommand(
        int ItemId, 
        int UsuarioReferenciaId
    ) : IRequest<bool>;
}