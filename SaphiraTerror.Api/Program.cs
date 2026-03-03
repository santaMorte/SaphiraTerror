//using SaphiraTerror.Infrastructure;
//using SaphiraTerror.Infrastructure.Persistence.Seed;

//var builder = WebApplication.CreateBuilder(args);

//// Infra (DbContext + Identity)
//builder.Services.AddInfrastructure(builder.Configuration);

//// Controllers (usaremos na Fase 3)
//builder.Services.AddControllers();

//// CORS aberto (fecharemos na Fase 3)
//builder.Services.AddCors(o => o.AddPolicy("Default", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

//var app = builder.Build();

//app.UseCors("Default");

//// Health na raiz
//app.MapGet("/", () => Results.Ok(new { name = "SaphiraTerror.Api", status = "ok" }));

//app.MapControllers();

//// ---- SEED (migrations + dados iniciais)
//await DatabaseSeeder.EnsureSeededAsync(app.Services);

//app.Run();

//refatorado fase 03
using SaphiraTerror.Infrastructure;
using SaphiraTerror.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// Infra (DbContext + Identity + Repos/Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS por configuração (dev: localhost do Web MVC)
var allowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
              ?? new[] { "https://localhost:5001", "http://localhost:5000" };

builder.Services.AddCors(o =>
    o.AddPolicy("Default", p => p.WithOrigins(allowed)
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()));

var app = builder.Build();

app.UseCors("Default");

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health na raiz
app.MapGet("/", () => Results.Ok(new { name = "SaphiraTerror.Api", status = "ok" }));

app.MapControllers();

// Migrate + Seed
await DatabaseSeeder.EnsureSeededAsync(app.Services);

app.Run();

