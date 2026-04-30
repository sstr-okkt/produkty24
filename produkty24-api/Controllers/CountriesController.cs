using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;

        public CountriesController(IDbConnectionFactory db)
        {
            _db = db;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CountryEntity>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<CountryEntity>("SELECT * FROM Countries");
            return Ok(entities);
        }
    }
}
