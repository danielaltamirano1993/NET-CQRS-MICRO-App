using MediatR;

namespace Microservicio.Items.API.Application.Commands
{
    public record CrearItemCommand(
        string   Titulo, 
        string   Descripcion, 
        DateTime FechaEntrega, 
        byte     Relevancia
    ) : IRequest<int>;
}
