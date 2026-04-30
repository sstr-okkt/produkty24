using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Produkty24_API.Db;
using Produkty24_API.Models;
using Produkty24_API.Models.DTO.Clients;
using Produkty24_API.Models.Entities;

namespace Produkty24_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IDbConnectionFactory _db;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ClientsController(IDbConnectionFactory db, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _db = db;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        [HttpGet]
        public async Task<ActionResult<PageInfo<AllClientsDto>>> GetAll([FromQuery] int page, int pageSize)
        {
            using var connection = _db.CreateConnection();
            var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Clients");
            var totalPages = PageInfo<object>.PagesCount(totalCount, pageSize);

            if (page < 1 || page > totalPages)
                return NotFound(new { Message = $"Page {page} does not exist! Total pages: {totalPages}" });

            var entities = await connection.QueryAsync<ClientEntity, CountryEntity, ShippingMethodEntity, ClientEntity>(
                @"SELECT cl.*, co.Id, co.Name, sm.Id, sm.Name FROM Clients cl
                  LEFT JOIN Countries co ON cl.CountryId = co.Id
                  LEFT JOIN ShippingMethods sm ON cl.ShippingMethodId = sm.Id
                  LIMIT @PageSize OFFSET @Offset",
                (cl, co, sm) => { cl.Country = co; cl.ShippingMethod = sm; return cl; },
                new { PageSize = pageSize, Offset = (page - 1) * pageSize });

            var clients = _mapper.Map<List<AllClientsDto>>(entities.ToList());
            return Ok(new PageInfo<AllClientsDto>(totalPages, page, clients));
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<AllClientsDto>>> GetAllList()
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<ClientEntity, CountryEntity, ShippingMethodEntity, ClientEntity>(
                @"SELECT cl.*, co.Id, co.Name, sm.Id, sm.Name FROM Clients cl
                  LEFT JOIN Countries co ON cl.CountryId = co.Id
                  LEFT JOIN ShippingMethods sm ON cl.ShippingMethodId = sm.Id",
                (cl, co, sm) => { cl.Country = co; cl.ShippingMethod = sm; return cl; });

            var clients = _mapper.Map<IEnumerable<AllClientsDto>>(entities);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientEditDto>> Get(int id)
        {
            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<ClientEntity>(
                "SELECT * FROM Clients WHERE Id = @Id", new { Id = id });

            if (entity == null)
                return NotFound(new { id });

            var client = _mapper.Map<ClientEditDto>(entity);
            return Ok(client);
        }

        [HttpGet("{id}/profile")]
        public async Task<ActionResult<ClientProfileDto>> GetProfile(int id)
        {
            using var connection = _db.CreateConnection();
            var entities = await connection.QueryAsync<ClientEntity, CountryEntity, ShippingMethodEntity, ClientEntity>(
                @"SELECT cl.*, co.Id, co.Name, sm.Id, sm.Name FROM Clients cl
                  LEFT JOIN Countries co ON cl.CountryId = co.Id
                  LEFT JOIN ShippingMethods sm ON cl.ShippingMethodId = sm.Id
                  WHERE cl.Id = @Id",
                (cl, co, sm) => { cl.Country = co; cl.ShippingMethod = sm; return cl; },
                new { Id = id });

            var entity = entities.FirstOrDefault();
            if (entity == null)
                return NotFound(new { id });

            var client = _mapper.Map<ClientProfileDto>(entity);
            client.OrdersQuantity = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Orders WHERE ClientId = @Id", new { Id = id });
            client.PaymentsTotal = await connection.ExecuteScalarAsync<float>(
                "SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE ClientId = @Id", new { Id = id });

            return Ok(client);
        }

        [HttpPost]
        public async Task<ActionResult<ClientEntity>> Create([FromBody] ClientCreateDto client)
        {
            if (!ModelState.IsValid)
                return BadRequest(client);

            using var connection = _db.CreateConnection();
            var newEntity = new ClientEntity();
            _mapper.Map(client, newEntity);
            newEntity.Date = _dateTimeProvider.UtcNow;

            var id = await connection.ExecuteScalarAsync<int>(
                @"INSERT INTO Clients (Date, Name, Nickname, Phone, Email, City, Address, PostalCode, Notes, ShippingMethodId, CountryId)
                  VALUES (@Date, @Name, @Nickname, @Phone, @Email, @City, @Address, @PostalCode, @Notes, @ShippingMethodId, @CountryId);
                  SELECT last_insert_rowid();", newEntity);
            newEntity.Id = id;

            return Ok(newEntity);
        }

        [HttpPut]
        public async Task<ActionResult<ClientEntity>> Edit([FromBody] ClientEditDto client)
        {
            if (!ModelState.IsValid)
                return BadRequest(client);

            using var connection = _db.CreateConnection();
            var entity = await connection.QuerySingleOrDefaultAsync<ClientEntity>(
                "SELECT * FROM Clients WHERE Id = @Id", new { Id = client.Id });

            if (entity == null)
                return NotFound(new { id = client.Id });

            _mapper.Map(client, entity);
            await connection.ExecuteAsync(
                @"UPDATE Clients SET Name = @Name, Nickname = @Nickname, Phone = @Phone, Email = @Email,
                  City = @City, Address = @Address, PostalCode = @PostalCode, Notes = @Notes,
                  ShippingMethodId = @ShippingMethodId, CountryId = @CountryId WHERE Id = @Id", entity);

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            using var connection = _db.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM Clients WHERE Id = @Id", new { Id = id });
            return Ok(id);
        }
    }
}
