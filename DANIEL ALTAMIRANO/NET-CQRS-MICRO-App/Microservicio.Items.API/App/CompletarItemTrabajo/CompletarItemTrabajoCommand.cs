using MediatR;

namespace Microservicio.Items.API.App.CompletarItemTrabajo
{
    public record CompletarItemTrabajoCommand(
        int ItemId, 
        int UsuarioReferenciaId
    ) : IRequest<bool>;
}