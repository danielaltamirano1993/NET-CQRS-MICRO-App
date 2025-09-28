using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microservicio.Usuarios.API.Application.Commands;

namespace Microservicio.Usuarios.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { UsuarioId = id });
        }
    }
}
