using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microservicio.Items.API.Domain
{
    public class UsuarioReferencia
    {
        [Key]
        public int UsuarioId { get; private set; }

        [MaxLength(100)]
        public string NombreUsuario { get; private set; } = string.Empty;

        public string Correo { get; private set; } = string.Empty;
        public int LimiteItems { get; private set; }
        public bool Activo { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime UltimaActualizacion { get; private set; }

        public int ItemsPendientes { get; internal set; }
        public int ItemsCompletados { get; internal set; }

        public ICollection<ItemTrabajo> ItemsAsignados { get; set; } = new List<ItemTrabajo>();
        public ICollection<HistorialAsignacion> Historiales { get; set; } = new List<HistorialAsignacion>();

        private UsuarioReferencia() { }

        public static UsuarioReferencia Crear(int usuarioId, string nombreUsuario, string correo, int limiteItems)
        {
            return new UsuarioReferencia
            {
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario,
                Correo = correo,
                LimiteItems = limiteItems > 0 ? limiteItems : 5,
                Activo = true,
                ItemsPendientes = 0,
                ItemsCompletados = 0,
                FechaRegistro = DateTime.UtcNow,
                UltimaActualizacion = DateTime.UtcNow
            };
        }

        public void Actualizar(string nuevoNombre, string nuevoCorreo, int nuevoLimite)
        {
            NombreUsuario = nuevoNombre;
            Correo = nuevoCorreo;
            LimiteItems = nuevoLimite > 0 ? nuevoLimite : 5;
            UltimaActualizacion = DateTime.UtcNow;
        }

        public void Inactivar()
        {
            if (Activo)
            {
                Activo = false;
                UltimaActualizacion = DateTime.UtcNow;
            }
        }

        public void Activar()
        {
            if (!Activo)
            {
                Activo = true;
                UltimaActualizacion = DateTime.UtcNow;
            }
        }
    }
}
