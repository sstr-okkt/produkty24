using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Payments;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;
        private readonly IDateTimeProvider dateTimeProvider;

        public PaymentsController(DataContext dataContext, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
            this.dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllPaymentsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            IQueryable<PaymentEntity> source = dataContext.Payments;
            var totalPages = PageInfo<Object>.PagesCount(source, pageSize);

            if (page < 1 || page > totalPages) {
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });
            }

            var entities = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var payments = mapper.Map<List<AllPaymentsDto>>(entities);

            var pageResponse = new PageInfo<AllPaymentsDto>(totalPages, page, payments);

            return Ok(pageResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentEditDto>> Get(int id)
        {
            var entity = await dataContext.Payments.FindAsync(id);

            if (entity == null) {
                return NotFound(new { id = id });
            }

            var payment = new PaymentEditDto();
            mapper.Map(entity, payment);

            return Ok(payment);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentEntity>> Create([FromBody] PaymentCreateDto payment)
        {
            if (!ModelState.IsValid) {
                return BadRequest(payment);
            }

            var newPaymentEntity = new PaymentEntity();
            mapper.Map(payment, newPaymentEntity);
            newPaymentEntity.Date = dateTimeProvider.UtcNow;

            await dataContext.Payments.AddAsync(newPaymentEntity);
            await dataContext.SaveChangesAsync();

            return Ok(newPaymentEntity);
        }

        [HttpPut]
        public async Task<ActionResult<PaymentEntity>> Edit([FromBody] PaymentEditDto payment)
        {
            if (!ModelState.IsValid) {
                return BadRequest(payment);
            }

            var entity = await dataContext.Payments.FindAsync(payment.Id);

            if (entity == null) {
                return NotFound(new { id = payment.Id });
            }

            entity.Amount = payment.Amount;
            entity.Notes = payment.Notes;
            await dataContext.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            dataContext.Remove(new PaymentEntity() { Id = id });
            await dataContext.SaveChangesAsync();

            return Ok(id);
        }
    }
}
