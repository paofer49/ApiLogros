using ApiLogros.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using ApiLogros.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiLogros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController : ControllerBase
    {
        private readonly ConexionBD _db;

        public ActividadesController(ConexionBD db)
        {
            _db = db;
        }

        // GET: api/<ActividadesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using var conexion = _db.ObtenerConexion();

            var actividades = await conexion.QueryAsync<Actividad>(
                "sp_ObtenerActividades",
                commandType: CommandType.StoredProcedure);

            return Ok(actividades);
        }

        // GET api/<ActividadesController>/5
        [HttpGet("{id}")]
        public async Task <IActionResult> Get(int id)
        {
            using var conexion = _db.ObtenerConexion();

            var actividad = await conexion.QueryFirstOrDefaultAsync<Actividad>(
                "sp_ObtenerActividadPorId",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            if (actividad == null)
                return NotFound("Actividad no encontrada");

            return Ok(actividad);
        }

        // POST api/<ActividadesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Actividad actividad)
        {
            using var conexion = _db.ObtenerConexion();

            await conexion.ExecuteAsync(
                "sp_InsertarActividad",
                new
                {
                    actividad.Nombre,
                    actividad.Descripcion,
                    actividad.Puntos
                },
                commandType: CommandType.StoredProcedure);

            return Ok("Actividad creada");
        }

        // PUT api/<ActividadesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Actividad actividad)
        {
            using var conexion = _db.ObtenerConexion();

            await conexion.ExecuteAsync(
                "sp_ActualizarActividad",
                new
                {
                    Id = id,
                    actividad.Nombre,
                    actividad.Descripcion,
                    actividad.Puntos,
                    actividad.Activa
                },
                commandType: CommandType.StoredProcedure);

            return Ok("Actividad actualizada");
        }

        // DELETE api/<ActividadesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conexion = _db.ObtenerConexion();

            await conexion.ExecuteAsync(
                "sp_EliminarActividad",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return Ok("Actividad Desactivada");
        }
    }
}
