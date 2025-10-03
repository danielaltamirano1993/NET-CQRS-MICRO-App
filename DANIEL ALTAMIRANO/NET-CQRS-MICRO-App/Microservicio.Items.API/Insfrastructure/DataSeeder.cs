using Microservicio.Items.API.App.Dto;
using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Microservicio.Items.API.Infrastructure
{
    public static class DataSeeder
    {
        public static async Task EnsureUsersSynchronizedAsync(
            IServiceProvider serviceProvider, 
            IConfiguration configuration
        )
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ItemDbContext>();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(DataSeeder));

            logger.LogInformation("Iniciando proceso de sincronización inicial de UsuarioReferencia...");

            var client = httpClientFactory.CreateClient("UsuariosApi");

            var baseUrl = configuration["ServiciosExternos:UsuariosApiUrl"]?.TrimEnd('/');
            var apiUrl = $"{baseUrl}/api/Usuario";

            try
            {
                var usuariosExternos = await client.GetFromJsonAsync<List<UsuarioSincronizacionManualDto>>(apiUrl);

                if (usuariosExternos == null || 
                    !usuariosExternos.Any())
                {
                    logger.LogWarning("No se recibieron usuarios del Microservicio de Usuarios o la lista está vacía.");
                    return;
                }

                var usuariosExistentes = await context.UsuarioReferencia.ToDictionaryAsync(u => u.UsuarioId);
                int countAdded = 0;
                int countUpdated = 0;

                foreach (var userDto in usuariosExternos)
                {
                    if (usuariosExistentes.TryGetValue(
                        userDto.UsuarioId, 
                        out var referenciaExistente
                        )
                    )
                    {
                        referenciaExistente.Actualizar(
                            userDto.NombreUsuario,
                            userDto.Correo,
                            referenciaExistente.LimiteItems
                        );

                        if (referenciaExistente.Activo != 
                            userDto.Activo
                        )
                        {
                            if (userDto.Activo)
                            {
                                referenciaExistente.Activar();
                            }
                            else
                            {
                                referenciaExistente.Inactivar();
                            }
                        }
                        countUpdated++;
                    }
                    else
                    {
                        var nuevaReferencia = UsuarioReferencia.Crear(
                            userDto.UsuarioId,
                            userDto.NombreUsuario,
                            userDto.Correo,
                            3
                        );

                        if (!userDto.Activo)
                        {
                            nuevaReferencia.Inactivar();
                        }

                        context.UsuarioReferencia.Add(nuevaReferencia);
                        countAdded++;
                    }
                }

                await context.SaveChangesAsync();

                logger.LogInformation(
                    $"✅ Sincronización inicial completada. " +
                    $"Añadidos: {countAdded}, " +
                    $"Actualizados: {countUpdated}");
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(
                    ex, 
                    $"❌ ERROR: No se pudo conectar o recibir datos del Microservicio de Usuarios en " +
                    $"{apiUrl}. Verifica la URL o el estado del servicio."
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex, 
                    "❌ ERROR: Fallo al procesar o guardar los usuarios en ItemDbContext."
                );
            }
        }
    }
}
