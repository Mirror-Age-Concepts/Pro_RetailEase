using Microsoft.Extensions.Diagnostics.HealthChecks;
using RetailEase.Api.Health;

namespace RetailEase.UnitTests.Api;

public class SelfHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthyResult_WhenHostIsReachable()
    {
        // Arrange
        var check = new SelfHealthCheck();
        var context = new HealthCheckContext();

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Fact]
    public async Task CheckHealthAsync_ReturnsHealthyResult_WithDescription()
    {
        // Arrange
        var check = new SelfHealthCheck();
        var context = new HealthCheckContext();

        // Act
        var result = await check.CheckHealthAsync(context);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result.Description));
    }
}
