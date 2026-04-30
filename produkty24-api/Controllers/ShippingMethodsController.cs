using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShippingMethodsController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public ShippingMethodsController(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ShippingMethodEntity>>> GetAllList()
        {
            var entities = await dataContext.ShippingMethods.ToListAsync();

            return Ok(entities);
        }
    }
}
