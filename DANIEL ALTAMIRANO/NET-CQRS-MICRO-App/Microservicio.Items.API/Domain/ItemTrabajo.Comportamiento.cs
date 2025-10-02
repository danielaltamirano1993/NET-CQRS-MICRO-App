namespace Microservicio.Items.API.Domain
{
    public partial class ItemTrabajo
    {
        public void AsignarUsuario(int usuarioId)
        {
            if (this.UsuarioAsignado != usuarioId)
            {
                this.UsuarioAsignado = usuarioId;
            }
        }

        public void AgregarHistorial(HistorialAsignacion historial)
        {
            if (historial != null)
            {
                // añadir historial (feature new)...
            }
        }

        public void IniciarTrabajo()
        {
            if (this.Estado == "Pendiente")
            {
                this.Estado = "Pendiente";
            }
        }
    }
}