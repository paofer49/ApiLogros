using ApiLogros.Data;
using ApiLogros.Model;
using System.Data;
using Dapper;

namespace ApiLogros.Servicios
{
    public class AsignacionActividadesService
    {
        private readonly ConexionBD _db;

        public AsignacionActividadesService(ConexionBD db)
        {
            _db = db;
        }

        public async Task AsignarActividadesAUsuario(int usuarioId)
        {
            using var conexion = _db.ObtenerConexion();

            int total = await conexion.ExecuteScalarAsync<int>(
                "sp_UsuarioTieneActividadesHoy",
                new { UsuarioId = usuarioId },
                commandType: CommandType.StoredProcedure);

            if (total > 0)
            {
                Console.WriteLine($"[Asignacion] Usuario {usuarioId} ya tiene actividades hoy.");
                return;
            }

            var actividades = await conexion.QueryAsync<Actividad>(
                "sp_Obtener5ActividadesRandom",
                commandType: CommandType.StoredProcedure);

            foreach (var actividad in actividades)
            {
                await conexion.ExecuteAsync(
                    "sp_AsignarActividadUsuario",
                    new { UsuarioId = usuarioId, ActividadId = actividad.IdActividad },
                    commandType: CommandType.StoredProcedure);
            }

            Console.WriteLine($"[Asignacion] Usuario {usuarioId} — actividades asignadas ✓");
        }
    }
}
