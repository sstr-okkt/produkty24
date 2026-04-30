using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Produkty24_API;
using DataContext = Produkty24_API.Db.DataContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionBuilder = new SqliteConnectionStringBuilder(connectionString);

if (!Path.IsPathRooted(connectionBuilder.DataSource))
{
    connectionBuilder.DataSource = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, connectionBuilder.DataSource));
}

var databaseDirectory = Path.GetDirectoryName(connectionBuilder.DataSource);
if (!string.IsNullOrWhiteSpace(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionBuilder.ToString()));
builder.Services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(StaticTokenAuthOptions.DefaultSchemeName)
    .AddScheme<StaticTokenAuthOptions, StaticTokenAuthHandler>(
        StaticTokenAuthOptions.DefaultSchemeName,
        opts => {
            // you can change the token header name here by :
            // opts.TokenHeaderName = "X-Custom-Token-Header";
        });

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{    
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
