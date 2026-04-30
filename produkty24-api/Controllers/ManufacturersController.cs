using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Manufacturers;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;

        public ManufacturersController(IDbConnectionFactory db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<ManufacturerEditDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Manufacturers");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<ManufacturerEntity>(
                "SELECT * FROM Manufacturers LIMIT @PageSize OFFSET @Offset",
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var manufacturers = _mapper.Map<List<ManufacturerEditDto>>(entities.ToList());
            return Ok(new PageInfo<ManufacturerEditDto>(totalPages, page, manufacturers));
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<ManufacturerEditDto>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<ManufacturerEntity>("SELECT * FROM Manufacturers");
            var manufacturers = _mapper.Map<IEnumerable<ManufacturerEditDto>>(entities);
            return Ok(manufacturers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<ManufacturerEntity>(
                "SELECT * FROM Manufacturers WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var manufacturer = _mapper.Map<ManufacturerEditDto>(entity);
            return Ok(manufacturer);
        }

        [HttpPost]
        public async Task<ActionResult<ManufacturerEntity>> Create([FromBody] ManufacturerCreateDto manufacturer)
        {
            if (!ModelState.IsValid)
                return BadRequest(manufacturer);

            using var connection = _db.CreateConnection();
            var newEntity = new ManufacturerEntity();
            _mapper.Map(manufacturer, newEntity);

            var id = await connection.ExecuteScalarAsync<int>(
                "INSERT INTO Manufacturers (Name) VALUES (@Name); SELECT last_insert_rowid();",
                newEntity);
            newEntity.Id = id;

            return Ok(newEntity);
        }

        [HttpPut]
        public async Task<ActionResult<ManufacturerEntity>> Edit([FromBody] ManufacturerEditDto manufacturer)
        {
            if (!ModelState.IsValid)
                return BadRequest(manufacturer);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<ManufacturerEntity>(
                "SELECT * FROM Manufacturers WHERE Id = @Id", new { Id = manufacturer.Id });

            if (entity == null)
                return NotFound(new { id = manufacturer.Id });

            _mapper.Map(manufacturer, entity);
            await connection.ExecuteAsync(
                "UPDATE Manufacturers SET Name = @Name WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Manufacturers WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
