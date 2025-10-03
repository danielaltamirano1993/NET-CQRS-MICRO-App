using MediatR;
using System;

namespace Microservicio.Items.API.App.Commands
{
    public record CrearItemTrabajoCommand : IRequest<int>
    {
        public string Titulo { get; init; } = string.Empty;
        public string Descripcion { get; init; } = string.Empty;
        public DateTime FechaEntrega { get; init; }
        public byte Relevancia { get; init; } = 1;
        public int? UsuarioAsignado { get; init; }

    }
}
