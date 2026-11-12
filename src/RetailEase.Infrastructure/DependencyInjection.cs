using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailEase.Infrastructure.Persistence;

namespace RetailEase.Infrastructure;

/// <summary>
/// Infrastructure composition root extensions.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RetailEaseDatabase");

        services.AddDbContext<RetailEaseDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
