using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using RetailEase.Api.Health;
using RetailEase.Application.Stores;
using RetailEase.Application.Transactions;
using RetailEase.Infrastructure;
using RetailEase.Infrastructure.Persistence;
using RetailEase.Infrastructure.Transactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SqliteDbContext>(options => options.UseSqlite("Data Source=local.db"));

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITransactionStore, SqliteTransactionStore>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();

builder.Services.AddHealthChecks()
    .AddCheck<SelfHealthCheck>("self", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapHealthChecks("/health");

app.Run();
