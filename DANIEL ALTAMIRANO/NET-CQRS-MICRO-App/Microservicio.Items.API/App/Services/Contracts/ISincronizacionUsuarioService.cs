using System.Threading.Tasks;
using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.App.Services.Contracts
{
    public interface ISincronizacionUsuarioService
    {
        Task SincronizarUsuariosAsync();
        Task<UsuarioReferencia?> ObtenerUsuarioConMenorCargaAsync();
    }
}