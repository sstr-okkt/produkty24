using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.ExchangeRates;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ExchangeRatesController(IDbConnectionFactory db, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllExchangeRatesDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ExchangeRates");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<ExchangeRateEntity, CurrencyEntity, ExchangeRateEntity>(
                @"SELECT er.*, c.Id, c.Code FROM ExchangeRates er
                  LEFT JOIN Currencies c ON er.CurrencyId = c.Id
                  LIMIT @PageSize OFFSET @Offset",
                (er, c) => { er.Currency = c; return er; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var exchangeRates = _mapper.Map<List<AllExchangeRatesDto>>(entities.ToList());
            return Ok(new PageInfo<AllExchangeRatesDto>(totalPages, page, exchangeRates));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExchangeRateEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<ExchangeRateEntity>(
                "SELECT * FROM ExchangeRates WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var exchangeRate = _mapper.Map<ExchangeRateEditDto>(entity);
            return Ok(exchangeRate);
        }

        [HttpGet("current/{id}")]
        public async Task<ActionResult<ExchangeRateEntity>> GetCurrent(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QueryFirstOrDefaultAsync<ExchangeRateEntity>(
                @"SELECT er.*, c.Id, c.Code FROM ExchangeRates er
                  LEFT JOIN Currencies c ON er.CurrencyId = c.Id
                  WHERE er.CurrencyId = @CurrencyId ORDER BY er.Id DESC LIMIT 1",
                new { CurrencyId = id });

            return Ok(entity);
        }

        [HttpPost]
        public async Task<ActionResult<ExchangeRateEntity>> Create([FromBody] ExchangeRateCreateDto exchangeRate)
        {
            if (!ModelState.IsValid)
                return BadRequest(exchangeRate);

            using var connection = _db.CreateConnection();
            var newEntity = new ExchangeRateEntity();
            _mapper.Map(exchangeRate, newEntity);
            newEntity.Date = _dateTimeProvider.UtcNow;

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO ExchangeRates (Date, CurrencyId, Value) VALUES (@Date, @CurrencyId, @Value);
                  SELECT last_insert_rowid();", newEntity);
            newEntity.Id = id;

            return Ok(newEntity);
        }

        [HttpPut]
        public async Task<ActionResult<ExchangeRateEntity>> Edit([FromBody] ExchangeRateEditDto exchangeRate)
        {
            if (!ModelState.IsValid)
                return BadRequest(exchangeRate);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<ExchangeRateEntity>(
                "SELECT * FROM ExchangeRates WHERE Id = @Id", new { Id = exchangeRate.Id });

            if (entity == null)
                return NotFound(new { id = exchangeRate.Id });

            _mapper.Map(exchangeRate, entity);
            await connection.ExecuteAsync(
                "UPDATE ExchangeRates SET Date = @Date, CurrencyId = @CurrencyId, Value = @Value WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM ExchangeRates WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
