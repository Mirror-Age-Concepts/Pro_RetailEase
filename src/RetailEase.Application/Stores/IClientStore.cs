using RetailEase.Domain.Entities;

namespace RetailEase.Application.Stores;

/// <summary>
/// Persistence abstraction for registered client devices.
/// </summary>
public interface IClientStore
{
    Task SaveAsync(ClientDevice client, CancellationToken cancellationToken = default);

    Task<ClientDevice?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
}
