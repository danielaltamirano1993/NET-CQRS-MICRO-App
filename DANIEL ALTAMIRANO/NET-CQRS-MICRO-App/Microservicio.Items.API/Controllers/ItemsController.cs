using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microservicio.Items.API.Application.Commands;
using Microservicio.Items.API.Application.Queries;

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

        [HttpPost]
        public async Task<IActionResult> Crear( 
            [FromBody] CrearItemCommand command
        )
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(
                nameof(ObtenerPendientesPorUsuario),
                new { 
                     usuarioId = 0 
                    }, 
                new { 
                     ItemId = id 
                    }
                );
        }

        [HttpPost("{itemId}/assign")]
        public async Task<IActionResult> Asignar(
            int itemId
        )
        {
            var result = await _mediator.Send(
                new AsignarItemCommand(itemId)
            );
            if (result) { 
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("pendientes/{usuarioId}")]
        public async Task<IActionResult> ObtenerPendientesPorUsuario(
            int usuarioId
        )
        {
            var items = await _mediator.Send(
                new GetPendientesPorUsuarioQuery(
                    usuarioId
                )
            );
            return Ok(items);
        }
    }
}
