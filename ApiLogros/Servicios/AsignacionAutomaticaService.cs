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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await AsignarActividades();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Esperar 24 horas
                var ahora = DateTime.Now;

                var siguienteMedianoche =
                    ahora.Date.AddDays(1);

                var tiempoEspera =
                    siguienteMedianoche - ahora;

                await Task.Delay(
                    tiempoEspera,
                    stoppingToken);
            }
        }

        private async Task AsignarActividades()
        {
            using var scope =_scopeFactory.CreateScope();

            var db =scope.ServiceProvider.GetRequiredService<ConexionBD>();

            using var conexion = db.ObtenerConexion();

            var cliente = _httpClientFactory.CreateClient("ApiUsuarios");

            // Obtener usuarios desde API Usuarios
            var usuarios =await cliente.GetFromJsonAsync<List<Usuario>>("Usuarios/interno");

            if (usuarios == null)
                return;

            foreach (var usuario in usuarios)
            {
                // Verificar si ya tiene actividades hoy
                int total =
                    await conexion.ExecuteScalarAsync<int>(
                        "sp_UsuarioTieneActividadesHoy",
                        new
                        {
                            UsuarioId = usuario.Id
                        },
                        commandType:
                            CommandType.StoredProcedure);

                if (total > 0)
                    continue;

                // Obtener 5 actividades random
                var actividades =
                    await conexion.QueryAsync<Actividad>(
                        "sp_Obtener5ActividadesRandom",
                        commandType:
                            CommandType.StoredProcedure);

                foreach (var actividad in actividades)
                {
                    await conexion.ExecuteAsync(
                        "sp_AsignarActividadUsuario",
                        new
                        {
                            UsuarioId = usuario.Id,
                            ActividadId = actividad.IdActividad
                        },
                        commandType:
                            CommandType.StoredProcedure);
                }
            }
        }
    }
}
