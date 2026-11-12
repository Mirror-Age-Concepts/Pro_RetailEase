using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;

namespace RetailEase.Infrastructure.Clients;

/// <summary>
/// In-memory implementation of <see cref="IClientStore"/> used for Client Manager slices
/// until a persistent client registration store is introduced.
/// </summary>
public class InMemoryClientStore : IClientStore
{
    private readonly Dictionary<string, ClientDevice> _clients = new(StringComparer.OrdinalIgnoreCase);

    public Task SaveAsync(ClientDevice client, CancellationToken cancellationToken = default)
    {
        _clients[client.Identifier] = client;

        return Task.CompletedTask;
    }

    public Task<ClientDevice?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_clients.TryGetValue(identifier, out var client) ? client : null);
    }
}
