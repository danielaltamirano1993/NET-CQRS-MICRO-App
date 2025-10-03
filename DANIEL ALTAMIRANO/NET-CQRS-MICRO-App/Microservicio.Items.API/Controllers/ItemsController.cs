using MediatR;
using Microservicio.Items.API.App.Commands;
using Microservicio.Items.API.App.Commands.CambiarEstadoItem;
using Microservicio.Items.API.App.Commands.CrearItemTrabajo;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.App.Queries.ConsultarTodosLosItems;
using Microservicio.Items.API.App.Queries.GetPendientesPorUsuario;
using Microservicio.Items.API.App.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Microservicio.Items.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISincronizacionUsuarioService _sincronizacionUsuarioService;

        public ItemsController(IMediator mediator, ISincronizacionUsuarioService sincronizacionUsuarioService)
        {
            _mediator = mediator;
            _sincronizacionUsuarioService = sincronizacionUsuarioService;
        }

        /// <summary>
        /// Crea un nuevo ítem de trabajo y lo asigna al usuario con menor carga.
        /// </summary>
        [HttpPost("CrearItemTrabajo")]
        public async Task<ActionResult<int>> CrearItemTrabajo(CrearItemTrabajoCommand command)
        {
            var itemId = await _mediator.Send(command);
            return Ok(itemId);
        }

        /// <summary>
        /// Cambia el estado de un ítem de trabajo existente.
        /// </summary>
        [HttpPut("{itemId}/CambiarEstado")]
        public async Task<ActionResult> CambiarEstadoItem( 
             int itemId,
             [FromBody] CambiarEstadoItemCommand command
        )
        {
            command = command with { ItemId = itemId };
            var result = await _mediator.Send(command); 

            if (!result) 
            {
                return NotFound($"Ítem con ID {itemId} no encontrado o estado no válido.");
            }
            return Ok(new
            {
                mensaje = "Estado del ítem cambiado exitosamente.",
                itemId = itemId,
                nuevoEstado = command.NuevoEstado 
            });
        }

        /// <summary>
        /// Consulta todos los ítems de trabajo pendientes para un usuario específico, ordenados por Relevancia y FechaEntrega.
        /// </summary>
        [HttpGet("PendientesPorUsuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<ItemTrabajoSqlResult>>> GetPendientesPorUsuario(int usuarioId)
        {
            // Corregido: Si el constructor requiere el UsuarioId como parámetro.
            var query = new GetPendientesPorUsuarioQuery(usuarioId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Consulta todos los ítems de trabajo en el sistema (Administrador).
        /// </summary>
        [HttpGet("ConsultarTodosLosItems")]
        public async Task<ActionResult<IEnumerable<ItemTrabajoDto>>> ConsultarTodosLosItems()
        {
            var query = new ConsultarTodosLosItemsQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        /// <summary>
        /// Dispara la sincronización manual de usuarios desde el microservicio externo. (Endpoint de Administración/Mantenimiento)
        /// </summary>
        [HttpPost("SincronizarUsuarios")]
        public async Task<ActionResult> SincronizarUsuarios()
        {
            var result = await _sincronizacionUsuarioService.SincronizarUsuariosAsync();

            if (result.Exito)
            {
                return Ok(new
                {
                    mensaje = result.MensajeDetalle ?? "Sincronización completada con éxito.",
                    resumen = result
                });
            }
            else
            {
                return StatusCode(500, new
                {
                    mensaje = result.MensajeDetalle ?? "La sincronización falló. Verifique los logs.",
                    resumen = result
                });
            }
        }

    }
}
