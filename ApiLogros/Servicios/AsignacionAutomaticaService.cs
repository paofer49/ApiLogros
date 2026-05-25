using ApiLogros.Data;
using ApiLogros.Model;
using System.Data;
using Dapper;

namespace ApiLogros.Servicios
{
    public class AsignacionAutomaticaService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;

        public AsignacionAutomaticaService(IServiceScopeFactory scopeFactory,IHttpClientFactory httpClientFactory)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await AsignarActividades();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inicial: {ex.Message}");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // Esperar 24 horas
                var ahora = DateTime.Now;

                var siguienteMedianoche = ahora.Date.AddDays(1);

                var tiempoEspera = siguienteMedianoche - ahora;

                await Task.Delay(tiempoEspera,stoppingToken);

                try
                {
                    await AsignarActividades();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private async Task AsignarActividades()
        {
            using var scope =_scopeFactory.CreateScope();

            var asignacionService = scope.ServiceProvider.GetRequiredService<AsignacionActividadesService>();

            var cliente = _httpClientFactory.CreateClient("ApiUsuarios");

            // Obtener usuarios desde API Usuarios
            var usuarios =await cliente.GetFromJsonAsync<List<Usuario>>("Usuarios/interno");

            Console.WriteLine($"[AsignacionAutomatica] Usuarios obtenidos: {usuarios?.Count ?? 0}");

            if (usuarios == null || !usuarios.Any())
            {
                Console.WriteLine("[AsignacionAutomatica] No hay usuarios, saliendo.");
                return;
            }

            foreach (var usuario in usuarios)
            {
                await asignacionService.AsignarActividadesAUsuario(usuario.Id);
            }
        }
    }
}
