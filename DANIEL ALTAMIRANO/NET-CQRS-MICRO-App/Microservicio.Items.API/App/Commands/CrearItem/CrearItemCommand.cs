using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Microservicio.Items.API.App.Commands.CrearItem
{
    public record CrearItemCommand : IRequest<int>
    {
        [Required]
        [MaxLength(100)]
        public string Titulo { get; init; } = string.Empty;

        public string Descripcion { get; init; } = string.Empty;
        public byte Relevancia { get; init; } = 1;

        public DateTime FechaEntrega { get; init; }
    }
}
