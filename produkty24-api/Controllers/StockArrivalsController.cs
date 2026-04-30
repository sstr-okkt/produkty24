using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.StockArrivals;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockArrivalsController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public StockArrivalsController(IDbConnectionFactory db, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllStockArrivalsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM StockArrivals");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<StockArrivalEntity, StockItemEntity, StockArrivalEntity>(
                @"SELECT sa.*, si.Id, si.Name FROM StockArrivals sa
                  LEFT JOIN StockItems si ON sa.StockItemId = si.Id
                  LIMIT @PageSize OFFSET @Offset",
                (sa, si) => { sa.StockItem = si; return sa; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var stockArrivals = _mapper.Map<List<AllStockArrivalsDto>>(entities.ToList());
            return Ok(new PageInfo<AllStockArrivalsDto>(totalPages, page, stockArrivals));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockArrivalEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<StockArrivalEntity>(
                "SELECT * FROM StockArrivals WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var stockArrival = _mapper.Map<StockArrivalEditDto>(entity);
            return Ok(stockArrival);
        }

        [HttpPost]
        public async Task<ActionResult<StockArrivalEntity>> Create([FromBody] StockArrivalCreateDto stockArrival)
        {
            if (!ModelState.IsValid)
                return BadRequest(stockArrival);

            using var connection = _db.CreateConnection();
            var newEntity = new StockArrivalEntity();
            _mapper.Map(stockArrival, newEntity);
            newEntity.Date = _dateTimeProvider.UtcNow;

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO StockArrivals (Date, StockItemId, Quantity) VALUES (@Date, @StockItemId, @Quantity);
                  SELECT last_insert_rowid();", newEntity);
            newEntity.Id = id;

            return Ok(newEntity);
        }

        [HttpPut]
        public async Task<ActionResult<StockArrivalEntity>> Edit([FromBody] StockArrivalEditDto stockArrival)
        {
            if (!ModelState.IsValid)
                return BadRequest(stockArrival);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<StockArrivalEntity>(
                "SELECT * FROM StockArrivals WHERE Id = @Id", new { Id = stockArrival.Id });

            if (entity == null)
                return NotFound(new { id = stockArrival.Id });

            _mapper.Map(stockArrival, entity);
            await connection.ExecuteAsync(
                "UPDATE StockArrivals SET Date = @Date, StockItemId = @StockItemId, Quantity = @Quantity WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM StockArrivals WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
