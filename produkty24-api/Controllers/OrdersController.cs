using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Orders;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public OrdersController(IDbConnectionFactory db, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<OrderDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Orders");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<OrderEntity, ClientEntity, OrderStatusEntity, OrderEntity>(
                @"SELECT o.*, cl.Id, cl.Name, os.Id, os.Name FROM Orders o
                  LEFT JOIN Clients cl ON o.ClientId = cl.Id
                  LEFT JOIN OrderStatuses os ON o.StatusId = os.Id
                  LIMIT @PageSize OFFSET @Offset",
                (o, cl, os) => { o.Client = cl; o.Status = os; return o; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var orders = _mapper.Map<List<OrderDto>>(entities.ToList());
            return Ok(new PageInfo<OrderDto>(totalPages, page, orders));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailsDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<OrderEntity, ClientEntity, OrderStatusEntity, OrderEntity>(
                @"SELECT o.*, cl.Id, cl.Name, os.Id, os.Name FROM Orders o
                  LEFT JOIN Clients cl ON o.ClientId = cl.Id
                  LEFT JOIN OrderStatuses os ON o.StatusId = os.Id
                  WHERE o.Id = @Id",
                (o, cl, os) => { o.Client = cl; o.Status = os; return o; },
                new { Id = id });

            var entity = entities.FirstOrDefault();
            if (entity == null)
                return NotFound(new { id });

            var order = _mapper.Map<OrderDetailsDto>(entity);

            order.Items = (await connection.QueryAsync<OrderItemEntity, StockItemEntity, OrderItemEntity>(
                @"SELECT oi.*, si.Id, si.Name FROM OrdersItems oi
                  LEFT JOIN StockItems si ON oi.StockItemId = si.Id
                  WHERE oi.OrderId = @OrderId",
                (oi, si) => { oi.StockItem = si; return oi; },
                new { OrderId = id })).ToList();
            order.Total = (float)await connection.ExecuteScalarAsync<double>(
                "SELECT COALESCE(SUM(Total), 0) FROM OrdersItems WHERE OrderId = @OrderId", new { OrderId = id });
            order.PaymentsTotal = await connection.ExecuteScalarAsync<float>(
                "SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE OrderId = @OrderId", new { OrderId = id });
            order.Debt = order.Total - order.PaymentsTotal;

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderEntity>> Create([FromBody] OrderCreateDto order)
        {
            if (!ModelState.IsValid)
                return BadRequest(order);

            using var connection = _db.CreateConnection();
            var newOrder = new OrderEntity();
            _mapper.Map(order, newOrder);
            newOrder.Date = _dateTimeProvider.UtcNow;
            newOrder.StatusId = 4;

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Orders (Date, ClientId, StatusId, Notes) VALUES (@Date, @ClientId, @StatusId, @Notes);
                  SELECT last_insert_rowid();", newOrder);
            newOrder.Id = id;

            return Ok(newOrder);
        }

        [HttpPut]
        public async Task<ActionResult<OrderEntity>> Edit([FromBody] OrderEditDto order)
        {
            if (!ModelState.IsValid)
                return BadRequest(order);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<OrderEntity>(
                "SELECT * FROM Orders WHERE Id = @Id", new { Id = order.Id });

            if (entity == null)
                return NotFound(new { id = order.Id });

            _mapper.Map(order, entity);
            await connection.ExecuteAsync(
                "UPDATE Orders SET Date = @Date, ClientId = @ClientId, StatusId = @StatusId, Notes = @Notes WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Orders WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
