using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;

namespace RetailEase.Application.Clients;

/// <summary>
/// Default in-process implementation of the Client Manager orchestration backed by the client store.
/// </summary>
public class ClientManager : IClientManager
{
    private readonly IClientStore _store;

    public ClientManager(IClientStore store)
    {
        _store = store;
    }

    public async Task<ClientDevice> RegisterAsync(RegisterClientRequest request, CancellationToken cancellationToken = default)
    {
        var existingClient = await _store.GetByIdentifierAsync(request.Identifier, cancellationToken);

        if (existingClient is not null)
        {
            return existingClient;
        }

        var client = ClientDevice.Create(request.DeviceType, request.Identifier);
        await _store.SaveAsync(client, cancellationToken);

        return client;
    }

    public async Task<ClientDevice?> GetAsync(string deviceIdentifier, CancellationToken cancellationToken = default)
    {
        return await _store.GetByIdentifierAsync(deviceIdentifier, cancellationToken);
    }

    public async Task<ClientDevice> HeartbeatAsync(string deviceIdentifier, CancellationToken cancellationToken = default)
    {
        var client = await _store.GetByIdentifierAsync(deviceIdentifier, cancellationToken);

        if (client is null)
        {
            throw new KeyNotFoundException($"Client device with identifier '{deviceIdentifier}' was not found.");
        }

        client.Heartbeat();

        await _store.SaveAsync(client, cancellationToken);

        return client;
    }
}
