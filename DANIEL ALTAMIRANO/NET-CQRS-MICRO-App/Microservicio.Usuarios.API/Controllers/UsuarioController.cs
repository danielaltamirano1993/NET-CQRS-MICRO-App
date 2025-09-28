using MediatR;
using Microservicio.Usuarios.API.App.Commands.CrearUsuario;
using Microservicio.Usuarios.API.App.Queries.ObtenerUsuarios;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> CrearUsuario(
            [FromBody] CrearUsuarioCommand command
        )
        {
            var id = await _mediator.Send(command);
            return Ok(
                new {UsuarioId = id}
            );
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var usuarios = await _mediator.Send(
                new ObtenerUsuariosQuery()
            );
            return Ok(usuarios);
        }
    }
}
