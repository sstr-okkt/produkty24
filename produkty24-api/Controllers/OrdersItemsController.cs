using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.OrdersItems;
using Produkty24_API.Models.Entities;
using Produkty24_API.Processors;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersItemsController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public OrdersItemsController(IDbConnectionFactory db, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllOrderItemsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM OrdersItems");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<OrderItemEntity, OrderEntity, ClientEntity, StockItemEntity, OrderItemEntity>(
                @"SELECT oi.*, o.Id, o.Date, o.ClientId, cl.Id, cl.Name, si.Id, si.Name FROM OrdersItems oi
                  LEFT JOIN Orders o ON oi.OrderId = o.Id
                  LEFT JOIN Clients cl ON o.ClientId = cl.Id
                  LEFT JOIN StockItems si ON oi.StockItemId = si.Id
                  LIMIT @PageSize OFFSET @Offset",
                (oi, o, cl, si) => { o.Client = cl; oi.Order = o; oi.StockItem = si; return oi; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var ordersItems = _mapper.Map<List<AllOrderItemsDto>>(entities.ToList());
            return Ok(new PageInfo<AllOrderItemsDto>(totalPages, page, ordersItems));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<OrderItemEntity>(
                "SELECT * FROM OrdersItems WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var orderItem = _mapper.Map<OrderItemEditDto>(entity);
            return Ok(orderItem);
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemCreateDto>> Create([FromBody] OrderItemCreateDto orderItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(orderItem);

            using var connection = _db.CreateConnection();

            var exchangeRates = await GetCurrentExchangeRatesAsync(connection);
            var entity = _mapper.Map<OrderItemEntity>(orderItem);

            entity.StockItem = await connection.QuerySingleOrDefaultAsync<StockItemEntity>(
                "SELECT * FROM StockItems WHERE Id = @Id", new { Id = entity.StockItemId });

            var orderItemStateProcessor = new OrderItemStateProcessor(exchangeRates);
            orderItemStateProcessor.Calculate(entity);

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO OrdersItems (OrderId, StockItemId, Quantity, Price, Discount, Total, Profit, Expenses, ExchangeRate)
                  VALUES (@OrderId, @StockItemId, @Quantity, @Price, @Discount, @Total, @Profit, @Expenses, @ExchangeRate);
                  SELECT last_insert_rowid();", entity);
            entity.Id = id;

            return Ok(orderItem);
        }

        [HttpPut]
        public async Task<ActionResult<OrderItemEntity>> Edit([FromBody] OrderItemEditDto orderItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(orderItem);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<OrderItemEntity>(
                "SELECT * FROM OrdersItems WHERE Id = @Id", new { Id = orderItem.Id });

            if (entity == null)
                return NotFound(new { id = orderItem.Id });

            // Load related StockItem for calculation
            entity.StockItem = await connection.QuerySingleOrDefaultAsync<StockItemEntity>(
                "SELECT * FROM StockItems WHERE Id = @Id", new { Id = entity.StockItemId });

            _mapper.Map(orderItem, entity);

            var orderItemStateProcessor = new OrderItemStateProcessor();
            orderItemStateProcessor.Calculate(entity);

            await connection.ExecuteAsync(
                @"UPDATE OrdersItems SET OrderId = @OrderId, StockItemId = @StockItemId, Quantity = @Quantity,
                  Price = @Price, Discount = @Discount, Total = @Total, Profit = @Profit,
                  Expenses = @Expenses, ExchangeRate = @ExchangeRate WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM OrdersItems WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }

        private async Task<IEnumerable<ExchangeRateEntity>> GetCurrentExchangeRatesAsync(System.Data.IDbConnection connection)
        {
            var currencies = await connection.QueryAsync<CurrencyEntity>("SELECT * FROM Currencies");
            var exchangeRates = new List<ExchangeRateEntity>();

            foreach (var currency in currencies)
            {
                var currentRate = await connection.QueryFirstOrDefaultAsync<ExchangeRateEntity>(
                    "SELECT * FROM ExchangeRates WHERE CurrencyId = @CurrencyId ORDER BY Id DESC LIMIT 1",
                    new { CurrencyId = currency.Id });

                if (currentRate != null)
                    exchangeRates.Add(currentRate);
                else
                    exchangeRates.Add(new ExchangeRateEntity
                    {
                        Id = -1,
                        Date = _dateTimeProvider.UtcNow,
                        CurrencyId = currency.Id,
                        Value = 1
                    });
            }

            return exchangeRates;
        }
    }
}
