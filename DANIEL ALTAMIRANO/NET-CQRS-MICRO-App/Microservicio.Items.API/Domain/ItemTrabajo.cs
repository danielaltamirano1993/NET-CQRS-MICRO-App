namespace Microservicio.Items.API.Domain
{
    public class ItemTrabajo
    {
        public int ItemId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public byte Relevancia { get; set; } // 1 baja,
                                             // 2 alta
        public string Estado { get; set; } // Pendiente,
                                           // En Proceso,
                                           // Completado

        // FK a UsuarioReferencia.UsuarioId (solo ID)                                            
        public int? UsuarioAsignado { get; set; } 
    }
}
