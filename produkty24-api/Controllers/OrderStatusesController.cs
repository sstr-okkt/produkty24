using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderStatusesController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;

        public OrderStatusesController(IDbConnectionFactory db)
        {
            _db = db;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<OrderStatusEntity>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<OrderStatusEntity>("SELECT * FROM OrderStatuses");
            return Ok(entities);
        }
    }
}
