using MediatR;
using Microservicio.Items.API.App.Commands.CambiarEstadoItem;
using Microservicio.Items.API.App.Commands.CompletarItemTrabajo;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.App.Queries.GetPendientesPorUsuario;
using Microservicio.Items.API.App.Commands;

using Microsoft.AspNetCore.Mvc;

namespace Microservicio.Items.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ItemsController(
            IMediator mediator
        ) => _mediator = mediator;

        [HttpPost("CrearItemTrabajo")]
        public async Task<IActionResult> CrearItemTrabajo(
            [FromBody] CrearItemTrabajoCommand command
        )
        {
            // El Command ahora incluye la lógica de asignación, saturación y urgencia.
            var id = await _mediator.Send(command);
            return Ok(
                new
                {
                    ItemId = id
                }
            );
        }

        [HttpPut("{itemId}/CompletarItem/{usuarioReferenciaId}")]
        public async Task<IActionResult> CompletarItem(
            int itemId,
            int usuarioReferenciaId
        )
        {
            var result = await _mediator.Send(
                new CompletarItemTrabajoCommand(
                    itemId,
                    usuarioReferenciaId
                )
            );

            if (!result)
                return NotFound(
                    new
                    {
                        Message = "Ítem no encontrado"
                    }
                );
            return Ok(
                new
                {
                    Message = "Ítem marcado como completado"
                }
            );
        }

        //Pendiente
        //Completado
        [HttpPut("{itemId}/CambiarEstado")]
        public async Task<IActionResult> CambiarEstado(
            int itemId,
            [FromBody] CambiarEstadoItemBody body
        )
        {
            var command = new CambiarEstadoItemCommand(
                itemId,
                body.NuevoEstado
            );

            var result = await _mediator.Send(
                command
            );

            if (!result)
                return NotFound(
                    $"No se encontró el item {itemId}"
                );

            return Ok(
                new
                {
                    Message = $"El estado del item {itemId} " +
                                $"fue cambiado a " +
                                $"{command.NuevoEstado}"
                }
            );
        }

        // 🛑 Nombre del endpoint más conciso y descriptivo
        [HttpGet("PendientesPorUsuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerPendientesPorUsuario(
            int usuarioId
        )
        {
            // La lógica de consulta (ordenamiento y filtro) ahora está en el Handler con LINQ.
            var items = await _mediator.Send(
                new GetPendientesPorUsuarioQuery(
                    usuarioId
                )
            );
            return Ok(items);
        }
    }
}
