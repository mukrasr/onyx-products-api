using Onyx.ProductsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddHealthChecks();
services.AddAuthorization();
services.AddAuthentication("Bearer").AddJwtBearer();
services.AddControllers();

services.AddSingleton<IProductsService, ProductsService>();

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapHealthChecks("/");
app.MapControllers();

app.Run();
