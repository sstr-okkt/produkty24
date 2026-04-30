using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Produkty24_API;
using Produkty24_API.Db;

var builder = WebApplication.CreateBuilder(args);

// Build connection string
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

var resolvedConnectionString = connectionBuilder.ToString();

// Register services
builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(resolvedConnectionString));
builder.Services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(StaticTokenAuthOptions.DefaultSchemeName)
    .AddScheme<StaticTokenAuthOptions, StaticTokenAuthHandler>(
        StaticTokenAuthOptions.DefaultSchemeName,
        opts => { });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
using (var connection = new SqliteConnection(resolvedConnectionString))
{
    DatabaseInitializer.Initialize(connection);
}

// Configure pipeline
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
