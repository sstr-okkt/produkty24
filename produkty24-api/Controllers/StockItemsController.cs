using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.StockItems;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockItemsController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;

        public StockItemsController(IDbConnectionFactory db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllStockItemsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM StockItems");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<StockItemEntity, ManufacturerEntity, CurrencyEntity, StockItemEntity>(
                @"SELECT si.*, m.Id, m.Name, c.Id, c.Code FROM StockItems si
                  LEFT JOIN Manufacturers m ON si.ManufacturerId = m.Id
                  LEFT JOIN Currencies c ON si.CurrencyId = c.Id
                  LIMIT @PageSize OFFSET @Offset",
                (si, m, c) => { si.Manufacturer = m; si.Currency = c; return si; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var stockItems = _mapper.Map<List<AllStockItemsDto>>(entities.ToList());

            // Calculate quantity for each stock item
            foreach (var stockItem in stockItems)
            {
                var inOrders = await connection.ExecuteScalarAsync<float>(
                    "SELECT COALESCE(SUM(Quantity), 0) FROM OrdersItems WHERE StockItemId = @Id", new { Id = stockItem.Id });
                var inArrivals = await connection.ExecuteScalarAsync<float>(
                    "SELECT COALESCE(SUM(Quantity), 0) FROM StockArrivals WHERE StockItemId = @Id", new { Id = stockItem.Id });
                stockItem.Quantity = inArrivals - inOrders;
            }

            return Ok(new PageInfo<AllStockItemsDto>(totalPages, page, stockItems));
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<StockItemEntity>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<StockItemEntity, ManufacturerEntity, CurrencyEntity, StockItemEntity>(
                @"SELECT si.*, m.Id, m.Name, c.Id, c.Code FROM StockItems si
                  LEFT JOIN Manufacturers m ON si.ManufacturerId = m.Id
                  LEFT JOIN Currencies c ON si.CurrencyId = c.Id",
                (si, m, c) => { si.Manufacturer = m; si.Currency = c; return si; });
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockItemEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<StockItemEntity>(
                "SELECT * FROM StockItems WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var stockItem = _mapper.Map<StockItemEditDto>(entity);
            return Ok(stockItem);
        }

        [HttpPost]
        public async Task<ActionResult<StockItemEntity>> Create([FromBody] StockItemCreateDto stockItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(stockItem);

            using var connection = _db.CreateConnection();
            var newEntity = new StockItemEntity();
            _mapper.Map(stockItem, newEntity);

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO StockItems (Name, ManufacturerId, Description, CurrencyId, PurchasePrice, RetailPrice)
                  VALUES (@Name, @ManufacturerId, @Description, @CurrencyId, @PurchasePrice, @RetailPrice);
                  SELECT last_insert_rowid();", newEntity);
            newEntity.Id = id;

            return Ok(newEntity);
        }

        [HttpPut]
        public async Task<ActionResult<StockItemEditDto>> Edit([FromBody] StockItemEditDto stockItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(stockItem);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<StockItemEntity>(
                "SELECT * FROM StockItems WHERE Id = @Id", new { Id = stockItem.Id });

            if (entity == null)
                return NotFound(new { id = stockItem.Id });

            _mapper.Map(stockItem, entity);
            await connection.ExecuteAsync(
                @"UPDATE StockItems SET Name = @Name, ManufacturerId = @ManufacturerId, Description = @Description,
                  CurrencyId = @CurrencyId, PurchasePrice = @PurchasePrice, RetailPrice = @RetailPrice WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM StockItems WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
