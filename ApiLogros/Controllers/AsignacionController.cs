using ApiLogros.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiLogros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignacionController : ControllerBase
    {
        private readonly AsignacionActividadesService _asignacionService;

        public AsignacionController(AsignacionActividadesService asignacionService)
        {
            _asignacionService = asignacionService;
        }

        [HttpPost("asignar-nuevo-usuario/{usuarioId}")]
        public async Task<IActionResult> AsignarNuevoUsuario(int usuarioId)
        {
            await _asignacionService.AsignarActividadesAUsuario(usuarioId);
            return Ok(new { mensaje = "Actividades asignadas correctamente" });
        }
    }
}
