using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace RetailEase.Api.Health;

/// <summary>
/// Reports the health of the local RetailEase API host process.
/// Future releases will extend this check with downstream dependencies
/// (database, transaction manager service, licensing service, etc.).
/// </summary>
public sealed class SelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = HealthCheckResult.Healthy("RetailEase API host is healthy.");

        return Task.FromResult(result);
    }
}
