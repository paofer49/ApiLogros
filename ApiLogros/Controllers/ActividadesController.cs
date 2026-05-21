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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ActividadesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ActividadesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ActividadesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
