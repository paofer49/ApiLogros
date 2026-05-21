using ApiLogros.Data;
using ApiLogros.Model;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiLogros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioActividadesController : ControllerBase
    {
        private readonly ConexionBD _db;

        private readonly IHttpClientFactory _httpClientFactory;

        public UsuarioActividadesController(ConexionBD db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        //[Authorize]
        [HttpPatch("completar/{id}")]
        public async Task<IActionResult> CompletarActividad(int id)
        {
            using var conexion = _db.ObtenerConexion();

            var resultado =
                await conexion
                .QueryFirstOrDefaultAsync
                <CompletarActividadResponse>("sp_CompletarActividad",
                    new { UsuarioActividadId = id },
                    commandType: CommandType.StoredProcedure);

            if (resultado == null)
                return BadRequest();

            var cliente =_httpClientFactory.CreateClient("ApiUsuarios");

            var response =await cliente.PatchAsync( $"Usuarios/{resultado.UsuarioId}/puntos?puntos={resultado.Puntos}",null);

            var contenido = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return BadRequest("No se pudieron actualizar puntos");

            return Ok("Actividad completada");
        }

        //[Authorize]
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerActividadesUsuario(int usuarioId)
        {
            using var conexion = _db.ObtenerConexion();

            var actividades =
                await conexion.QueryAsync<UsuarioActividadDetalle>("sp_ObtenerActividadesUsuario",
                    new{UsuarioId = usuarioId},commandType: CommandType.StoredProcedure);

            return Ok(actividades);
        }

        [HttpGet("usuariohistorial/{usuarioId}")]
        public async Task<IActionResult> ObtenerHistorial(int usuarioId)
        {
            using var conexion = _db.ObtenerConexion();

            var historial = await conexion.QueryAsync<HistorialPuntos>(
                "sp_HistorialPuntosUsuario",
                new { UsuarioId = usuarioId },
                commandType: CommandType.StoredProcedure);

            return Ok(historial);
        }

        [HttpPost("asignar")]
        public async Task<IActionResult> AsignarActividad(int usuarioId, int actividadId)
        {
            var cliente = _httpClientFactory.CreateClient("ApiUsuarios");

            var response = await cliente.GetAsync($"usuarios/{usuarioId}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Usuario no existe");
            }

            using var conexion = _db.ObtenerConexion();

            await conexion.ExecuteAsync("sp_AsignarActividadUsuario",
                new
                {
                    UsuarioId = usuarioId,
                    ActividadId = actividadId
                },
                commandType: CommandType.StoredProcedure);

            return Ok("Actividad asignada");
        }
    }
}
