using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Manufacturers;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturersController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public ManufacturersController(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<ManufacturerEditDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            IQueryable<ManufacturerEntity> source = dataContext.Manufacturers;
            var totalPages = PageInfo<Object>.PagesCount(source, pageSize);

            if (page < 1 || page > totalPages)
            {
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });
            }

            var entities = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var manufacturers = mapper.Map<List<ManufacturerEditDto>>(entities);

            var pageResponse = new PageInfo<ManufacturerEditDto>(totalPages, page, manufacturers);

            return Ok(pageResponse);
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ManufacturerEditDto>>> GetAllList()
        {
            var entities = await dataContext.Manufacturers.ToListAsync();
            var manufacturers = mapper.Map<IEnumerable<ManufacturerEditDto>>(entities);

            return Ok(manufacturers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerEditDto>> Get(int id)
        {
            var entity = await dataContext.Manufacturers.FindAsync(id);

            if (entity == null) {
                return NotFound(new { id = id });
            }

            var manufacturer = new ManufacturerEditDto();
            mapper.Map(entity, manufacturer);

            return Ok(manufacturer);
        }

        [HttpPost]
        public async Task<ActionResult<ManufacturerEntity>> Create([FromBody] ManufacturerCreateDto manufacturer)
        {
            if (!ModelState.IsValid) {
                return BadRequest(manufacturer);
            }

            var newManufacturer = new ManufacturerEntity();
            mapper.Map(manufacturer, newManufacturer);

            await dataContext.Manufacturers.AddAsync(newManufacturer);
            await dataContext.SaveChangesAsync();

            return Ok(newManufacturer);
        }

        [HttpPut]
        public async Task<ActionResult<ManufacturerEntity>> Edit([FromBody] ManufacturerEditDto manufacturer)
        {
            if (!ModelState.IsValid) {
                return BadRequest(manufacturer);
            }

            var entity = await dataContext.Manufacturers.FindAsync(manufacturer.Id);

            if (entity == null) {
                return NotFound(new { id = manufacturer.Id });
            }

            mapper.Map(manufacturer, entity);
            await dataContext.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            dataContext.Remove(new ManufacturerEntity() { Id = id });
            await dataContext.SaveChangesAsync();

            return Ok(id);
        }
    }
}
