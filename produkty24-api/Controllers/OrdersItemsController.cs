using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.OrdersItems;
using Produkty24_API.Models.Entities;
using Produkty24_API.Processors;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersItemsController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public OrdersItemsController(DataContext dataContext, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
            this.dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllOrderItemsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            IQueryable<OrderItemEntity> source = dataContext.OrdersItems;
            var totalPages = PageInfo<Object>.PagesCount(source, pageSize);

            if (page < 1 || page > totalPages)
            {
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });
            }

            var entities = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var ordersItems = mapper.Map<List<AllOrderItemsDto>>(entities);

            var pageResponse = new PageInfo<AllOrderItemsDto>(totalPages, page, ordersItems);

            return Ok(pageResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemEditDto>> Get(int id)
        {
            var entity = await dataContext.OrdersItems.FindAsync(id);

            if (entity == null)
            {
                return NotFound(new { id = id });
            }

            var orderItem = new OrderItemEditDto();
            mapper.Map(entity, orderItem);

            return Ok(orderItem);
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemEntity>> Create([FromBody] OrderItemCreateDto orderItem)
        {
            if (!ModelState.IsValid) {
                return BadRequest(orderItem);
            }

            var exchangeRates = await GetCurrentExchangeRatesAsync();
            var entity = mapper.Map<OrderItemEntity>(orderItem);

            entity.StockItem = await dataContext.StockItems.FindAsync(entity.StockItemId);

            var orderItemStateProcessor = new OrderItemStateProcessor(exchangeRates);
            orderItemStateProcessor.Calculate(entity);

            await dataContext.OrdersItems.AddAsync(entity);
            await dataContext.SaveChangesAsync();

            return Ok(orderItem);
        }

        [HttpPut]
        public async Task<ActionResult<OrderItemEntity>> Edit([FromBody] OrderItemEditDto orderItem)
        {
            if (!ModelState.IsValid) {
                return BadRequest(orderItem);
            }

            var entity = await dataContext.OrdersItems.FindAsync(orderItem.Id);

            if (entity == null) {
                return NotFound(new { id = orderItem.Id });
            }

            mapper.Map(orderItem, entity);

            var orderItemStateProcessor = new OrderItemStateProcessor();
            orderItemStateProcessor.Calculate(entity);

            await dataContext.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            dataContext.Remove(new OrderItemEntity() { Id = id });
            await dataContext.SaveChangesAsync();

            return Ok(id);
        }

        private async Task<IEnumerable<ExchangeRateEntity>> GetCurrentExchangeRatesAsync()
        {
            var currencies = await dataContext.Currencies.ToListAsync();
            var exchangeRates = new List<ExchangeRateEntity>();

            foreach (var currency in currencies)
            {
                var currentExchangeRate = await dataContext.ExchangeRates
                    .Where(e => e.CurrencyId == currency.Id)
                    .OrderByDescending(e => e.Id)
                    .FirstOrDefaultAsync();

                if (currentExchangeRate != null)
                    exchangeRates.Add(currentExchangeRate);

                else
                {
                    var defaultExchangeRate = new ExchangeRateEntity()
                    {
                        Id = -1,
                        Date = dateTimeProvider.UtcNow,
                        CurrencyId = currency.Id,
                        Value = 1
                    };

                    exchangeRates.Add(defaultExchangeRate);
                }
            }

            return exchangeRates;
        }
    }
}
