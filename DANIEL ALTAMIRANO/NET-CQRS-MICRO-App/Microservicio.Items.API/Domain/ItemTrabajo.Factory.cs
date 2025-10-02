using System;

namespace Microservicio.Items.API.Domain
{
    public partial class ItemTrabajo
    {
        public static ItemTrabajo Crear(
            string titulo, 
            string descripcion, 
            DateTime fechaEntrega, 
            byte relevancia
        )
        {
            return new ItemTrabajo
            {
                Titulo = titulo,
                Descripcion = descripcion,
                FechaEntrega = fechaEntrega,
                Relevancia = relevancia,
                FechaCreacion = DateTime.UtcNow,
                Estado = "Pendiente"
            };
        }
    }
}