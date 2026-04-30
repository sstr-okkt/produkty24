using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Payments;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PaymentsController(IDbConnectionFactory db, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllPaymentsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Payments");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<PaymentEntity, ClientEntity, OrderEntity, PaymentEntity>(
                @"SELECT p.*, cl.Id, cl.Name, o.Id, o.Date FROM Payments p
                  LEFT JOIN Clients cl ON p.ClientId = cl.Id
                  LEFT JOIN Orders o ON p.OrderId = o.Id
                  LIMIT @PageSize OFFSET @Offset",
                (p, cl, o) => { p.Client = cl; p.Order = o; return p; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var payments = _mapper.Map<List<AllPaymentsDto>>(entities.ToList());
            return Ok(new PageInfo<AllPaymentsDto>(totalPages, page, payments));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<PaymentEntity>(
                "SELECT * FROM Payments WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var payment = _mapper.Map<PaymentEditDto>(entity);
            return Ok(payment);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentEntity>> Create([FromBody] PaymentCreateDto payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(payment);

            using var connection = _db.CreateConnection();
            var newEntity = new PaymentEntity();
            _mapper.Map(payment, newEntity);
            newEntity.Date = _dateTimeProvider.UtcNow;

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Payments (Date, ClientId, OrderId, Amount, Notes) VALUES (@Date, @ClientId, @OrderId, @Amount, @Notes);
                  SELECT last_insert_rowid();", newEntity);
            newEntity.Id = id;

            return Ok(newEntity);
        }

        [HttpPut]
        public async Task<ActionResult<PaymentEntity>> Edit([FromBody] PaymentEditDto payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(payment);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<PaymentEntity>(
                "SELECT * FROM Payments WHERE Id = @Id", new { Id = payment.Id });

            if (entity == null)
                return NotFound(new { id = payment.Id });

            entity.Amount = payment.Amount;
            entity.Notes = payment.Notes;
            await connection.ExecuteAsync(
                "UPDATE Payments SET Amount = @Amount, Notes = @Notes WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Payments WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
