using BagApi.Data;
using BagApi.Endpoints;
using SQLitePCL;
using WebApplication1.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

Batteries.Init();

var connString = builder.Configuration.GetConnectionString("Bag");
builder.Services.AddSqlite<BagContext>(connString);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapBagsEndpoints();
app.MapBrandsEndpoints();

await app.MigrateAsyncDb();

app.Run();