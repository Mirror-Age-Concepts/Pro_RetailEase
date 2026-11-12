using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using RetailEase.Api.Health;
using RetailEase.Api.Services;
using RetailEase.Application.Clients;
using RetailEase.Application.Synchronization;
using RetailEase.Application.Stores;
using RetailEase.Application.Transactions;
using RetailEase.Infrastructure;
using RetailEase.Infrastructure.Clients;
using RetailEase.Infrastructure.Persistence;
using RetailEase.Infrastructure.Synchronization;
using RetailEase.Infrastructure.Transactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SqliteDbContext>(options => options.UseSqlite("Data Source=local.db"));

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITransactionStore, SqliteTransactionStore>();
builder.Services.AddScoped<ITransactionManager, TransactionManager>();
builder.Services.AddSingleton<ICloudTransactionClient, InMemoryCloudTransactionClient>();
builder.Services.AddScoped<ISynchronizationEngine, TransactionSynchronizer>();
builder.Services.AddScoped<IClientStore, InMemoryClientStore>();
builder.Services.AddScoped<IClientManager, ClientManager>();
builder.Services.AddHostedService<TransactionSyncWorker>();

builder.Services.AddHealthChecks()
    .AddCheck<SelfHealthCheck>("self", failureStatus: HealthStatus.Unhealthy, tags: new[] { "ready" });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
    database.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapHealthChecks("/health");

app.Run();
