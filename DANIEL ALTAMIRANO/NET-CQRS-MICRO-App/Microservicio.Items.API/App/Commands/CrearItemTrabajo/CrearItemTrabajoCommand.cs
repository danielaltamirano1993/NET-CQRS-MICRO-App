using MediatR;
using System;

namespace Microservicio.Items.API.App.Commands
{
    public class CrearItemTrabajoCommand : IRequest<int>
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public byte Relevancia { get; set; } = 1; // 1 = Baja, 2 = Alta
    }
}
