using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models.DTO.Clients;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderStatusesController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public OrderStatusesController(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<AllClientsDto>>> GetAllList()
        {
            var entities = await dataContext.OrderStatuses.ToListAsync();

            return Ok(entities);
        }
    }
}
