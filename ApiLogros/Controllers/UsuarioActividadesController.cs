using ApiLogros.Data;
using ApiLogros.Model;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http.Headers;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiLogros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioActividadesController : ControllerBase
    {
        private readonly ConexionBD _db;

        private readonly IHttpClientFactory _httpClientFactory;

        private int GetIdActual() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private bool EsAdmin() => User.IsInRole("Administrador");

        public UsuarioActividadesController(ConexionBD db, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CrearClienteConToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var cliente = _httpClientFactory.CreateClient("ApiUsuarios");
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return cliente;
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

            var cliente = CrearClienteConToken();

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
            if (!EsAdmin() && GetIdActual() != usuarioId)
                return Forbid();

            using var conexion = _db.ObtenerConexion();

            var actividades =
                await conexion.QueryAsync<UsuarioActividadDetalle>("sp_ObtenerActividadesUsuario",
                    new{UsuarioId = usuarioId},commandType: CommandType.StoredProcedure);

            return Ok(actividades);
        }

        [HttpGet("usuariohistorial/{usuarioId}")]
        public async Task<IActionResult> ObtenerHistorial(int usuarioId)
        {
            if (!EsAdmin() && GetIdActual() != usuarioId)
                return Forbid();

            using var conexion = _db.ObtenerConexion();

            var historial = await conexion.QueryAsync<HistorialPuntos>(
                "sp_HistorialPuntosUsuario",
                new { UsuarioId = usuarioId },
                commandType: CommandType.StoredProcedure);

            return Ok(historial);
        }
    }
}
