using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Orders;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public OrdersController(DataContext dataContext, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
            this.dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<OrderDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            IQueryable<OrderEntity> source = dataContext.Orders;
            var totalPages = PageInfo<Object>.PagesCount(source, pageSize);

            if (page < 1 || page > totalPages)
            {
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });
            }

            var entities = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var orders = mapper.Map<List<OrderDto>>(entities);

            var pageResponse = new PageInfo<OrderDto>(totalPages, page, orders);

            return Ok(pageResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetailsDto>> Get(int id)
        {
            var entity = await dataContext.Orders.FindAsync(id);

            if (entity == null) {
                return NotFound(new { id = id });
            }

            var order = new OrderDetailsDto();
            mapper.Map(entity, order);

            order.Items = await dataContext.OrdersItems.Where(i => i.OrderId == id).ToListAsync();
            order.Total = (float)await dataContext.OrdersItems.Where(i => i.OrderId == id).SumAsync(i => i.Total);
            order.PaymentsTotal = await dataContext.Payments.Where(p => p.OrderId == id).SumAsync(p => p.Amount);
            order.Debt = order.Total - order.PaymentsTotal;

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderEntity>> Create([FromBody] OrderCreateDto order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(order);
            }

            var newOrder = new OrderEntity();
            mapper.Map(order, newOrder);
            newOrder.Date = dateTimeProvider.UtcNow;
            newOrder.StatusId = 4; 

            await dataContext.Orders.AddAsync(newOrder);
            await dataContext.SaveChangesAsync();

            return Ok(newOrder);
        }

        [HttpPut]
        public async Task<ActionResult<OrderEntity>> Edit([FromBody] OrderEditDto order)
        {
            if (!ModelState.IsValid) {
                return BadRequest(order);
            }

            var entity = await dataContext.Orders.FindAsync(order.Id);

            if (entity == null) {
                return NotFound(new { id = order.Id });
            }

            mapper.Map(order, entity);
            await dataContext.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            dataContext.Remove(new OrderEntity() { Id = id });
            await dataContext.SaveChangesAsync();

            return Ok(id);
        }
    }
}
