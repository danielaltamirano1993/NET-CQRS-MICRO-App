using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Domain;

public class SincronizacionUsuarioService : ISincronizacionUsuarioService
{
    private readonly HttpClient _httpClient;
    private const string UsuariosGetAllEndpoint = "/api/Usuario";

    public SincronizacionUsuarioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task<List<UsuarioExternoDto>?> ObtenerUsuariosExternosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(UsuariosGetAllEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Error al consumir el servicio externo de usuarios: {(int)response.StatusCode} ({response.ReasonPhrase}). Contenido: {errorContent}"
                );
            }

            return await response.Content.ReadFromJsonAsync<List<UsuarioExternoDto>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR CLIENTE USUARIOS]: {ex.Message}");
            throw;
        }
    }

    public async Task<SincronizacionResultDto> SincronizarUsuariosAsync()
    {
        var fechaEjecucion = DateTime.UtcNow;

        try
        {
            var usuariosExternos = await ObtenerUsuariosExternosAsync();

            if (usuariosExternos == null)
            {
                return new SincronizacionResultDto(
                    false,
                    0,
                    0,
                    0,
                    fechaEjecucion,
                    "El servicio de usuarios devolvió una respuesta nula."
                );
            }

            return new SincronizacionResultDto(
                true,
                usuariosExternos.Count,
                usuariosExternos.Count(u => u.Activo),
                0,
                fechaEjecucion,
                $"Sincronización exitosa. {usuariosExternos.Count} usuarios procesados."
            );
        }
        catch (Exception ex)
        {
            return new SincronizacionResultDto(
                false,
                0,
                0,
                0,
                fechaEjecucion,
                ex.Message
            );
        }
    }

    public Task<UsuarioReferencia?> ObtenerUsuarioConMenorCargaAsync()
    {
        var mockUsuario = UsuarioReferencia.Crear(
            usuarioId: 101,
            nombreUsuario: "UsuarioCargaBaja",
            correo: "carga.baja@mock.com",
            limiteItems: 50
        );

        mockUsuario.ItemsPendientes = 10;
        mockUsuario.ItemsCompletados = 5;

        return Task.FromResult<UsuarioReferencia?>(mockUsuario);
    }
}
