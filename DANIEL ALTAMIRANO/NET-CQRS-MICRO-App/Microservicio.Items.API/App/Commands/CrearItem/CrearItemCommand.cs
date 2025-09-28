using MediatR;

namespace Microservicio.Items.API.App.Commands.CrearItem
{
    public record CrearItemCommand(
        string   Titulo, 
        string   Descripcion, 
        DateTime FechaEntrega, 
        bool     Relevancia
    ) : IRequest<int>;
}
