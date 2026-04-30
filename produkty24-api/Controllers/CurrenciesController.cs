using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;

        public CurrenciesController(IDbConnectionFactory db)
        {
            _db = db;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CurrencyEntity>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<CurrencyEntity>("SELECT * FROM Currencies");
            return Ok(entities);
        }
    }
}
