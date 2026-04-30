using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.ExchangeRates;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public ExchangeRatesController(DataContext dataContext, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
            this.dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllExchangeRatesDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            IQueryable<ExchangeRateEntity> source = dataContext.ExchangeRates;
            var totalPages = PageInfo<Object>.PagesCount(source, pageSize);

            if (page < 1 || page > totalPages)
            {
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });
            }

            var entities = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var exchangeRates = mapper.Map<List<AllExchangeRatesDto>>(entities);

            var pageResponse = new PageInfo<AllExchangeRatesDto>(totalPages, page, exchangeRates);

            return Ok(pageResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExchangeRateEditDto>> Get(int id)
        {
            var entity = await dataContext.ExchangeRates.FindAsync(id);

            if (entity == null) {
                return NotFound(new { id = id });
            }

            var exchangeRate = new ExchangeRateEditDto();
            mapper.Map(entity, exchangeRate);

            return Ok(exchangeRate);
        }

        [HttpGet("current/{id}")]
        public async Task<ActionResult<ExchangeRateEntity>> GetCurrent(int id)
        {
            var currentExchangeRate = await dataContext.ExchangeRates
                    .Where(e => e.CurrencyId == id)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefaultAsync();

            return Ok(currentExchangeRate);
        }

        [HttpPost]
        public async Task<ActionResult<ExchangeRateEntity>> Create([FromBody] ExchangeRateCreateDto exchangeRate)
        {
            if (!ModelState.IsValid) {
                return BadRequest(exchangeRate);
            }

            var newExchangeRateEntity = new ExchangeRateEntity();
            mapper.Map(exchangeRate, newExchangeRateEntity);
            newExchangeRateEntity.Date = dateTimeProvider.UtcNow;

            await dataContext.ExchangeRates.AddAsync(newExchangeRateEntity);
            await dataContext.SaveChangesAsync();

            return Ok(newExchangeRateEntity);
        }

        [HttpPut]
        public async Task<ActionResult<ExchangeRateEntity>> Edit([FromBody] ExchangeRateEditDto exchangeRate)
        {
            if (!ModelState.IsValid) {
                return BadRequest(exchangeRate);
            }

            var entity = await dataContext.ExchangeRates.FindAsync(exchangeRate.Id);

            if (entity == null) {
                return NotFound(new { id = exchangeRate.Id });
            }

            mapper.Map(exchangeRate, entity);
            await dataContext.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            dataContext.Remove(new ExchangeRateEntity() { Id = id });
            await dataContext.SaveChangesAsync();

            return Ok(id);
        }
    }
}
