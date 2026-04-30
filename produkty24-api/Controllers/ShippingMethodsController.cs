using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingMethodsController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;

        public ShippingMethodsController(IDbConnectionFactory db)
        {
            _db = db;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ShippingMethodEntity>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<ShippingMethodEntity>("SELECT * FROM ShippingMethods");
            return Ok(entities);
        }
    }
}
