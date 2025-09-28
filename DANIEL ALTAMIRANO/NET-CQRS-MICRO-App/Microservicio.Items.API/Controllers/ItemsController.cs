using MediatR;
using Microservicio.Items.API.App.Commands.AsignarItem;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.App.Queries.GetPendientesPorUsuario;
using Microservicio.Items.API.Application.Commands;
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
            var id = await _mediator.Send(command);
            return Ok(
                new { 
                    ItemId = id 
                    }
            );
        }

        [HttpPost("AsignarItem/{usuarioId}")]
        public async Task<IActionResult> Asignar(
            int usuarioId,
            [FromBody] AsignarItemRequest request
        )
        {
            var result = await _mediator.Send(
                new AsignarItemCommand(usuarioId, request.ItemId)
            );

            if (result) return Ok();
            return BadRequest();
        }


        [HttpGet("ListarPendientesPorUsuario/{usuarioId}")]
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
