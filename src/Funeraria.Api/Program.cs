using System.Text.Json.Serialization;
using Funeraria.Infrastructure;
using Funeraria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddFunerariaInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema Funerário API",
        Version = "v1",
        Description = "API de cadastro de clientes (PF/PJ), filiais, serviços funerários e contratações com processamento assíncrono via fila in-memory + BackgroundService."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Funerário API v1");
    o.RoutePrefix = "swagger";
});

app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment() && app.Configuration.GetConnectionString("Oracle") is { Length: > 0 })
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FunerariaDbContext>();
    try { db.Database.Migrate(); }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Falha ao aplicar migrations");
    }
}

app.Run();

public partial class Program { }
