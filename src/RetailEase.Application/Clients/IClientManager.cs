using RetailEase.Domain.Entities;

namespace RetailEase.Application.Clients;

/// <summary>
/// Application-level orchestration for the Client Manager: device registration and health reporting.
/// </summary>
public interface IClientManager
{
    Task<ClientDevice> RegisterAsync(RegisterClientRequest request, CancellationToken cancellationToken = default);

    Task<ClientDevice?> GetAsync(string deviceIdentifier, CancellationToken cancellationToken = default);

    Task<ClientDevice> HeartbeatAsync(string deviceIdentifier, CancellationToken cancellationToken = default);
}
