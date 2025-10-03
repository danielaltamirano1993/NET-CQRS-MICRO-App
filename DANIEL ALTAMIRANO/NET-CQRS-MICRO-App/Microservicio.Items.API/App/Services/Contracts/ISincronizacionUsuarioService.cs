using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Domain;
using System.Threading.Tasks;

namespace Microservicio.Items.API.App.Services.Contracts
{
    public interface ISincronizacionUsuarioService
    {
        Task<SincronizacionResultDto> SincronizarUsuariosAsync();
        Task<UsuarioReferencia?> ObtenerUsuarioConMenorCargaAsync();
    }
}
