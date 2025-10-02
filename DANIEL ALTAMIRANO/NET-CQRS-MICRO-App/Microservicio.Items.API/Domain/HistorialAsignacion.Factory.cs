using System;

namespace Microservicio.Items.API.Domain
{
    public partial class HistorialAsignacion
    {
        public static HistorialAsignacion Crear(
            int itemTrabajoId, 
            int usuarioId, 
            string comentarios
        )
        {
            return new HistorialAsignacion
            {
                ItemTrabajoId = itemTrabajoId,
                UsuarioId = usuarioId,
                Comentarios = comentarios,
                FechaAsignacion = DateTime.UtcNow 
            };
        }
    }
}