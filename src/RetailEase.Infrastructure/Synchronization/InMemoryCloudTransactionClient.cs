using RetailEase.Application.Synchronization;
using RetailEase.Domain.Entities;

namespace RetailEase.Infrastructure.Synchronization;

/// <summary>
/// Phase 2 stand-in implementation of the cloud transaction client used by the
/// in-store Transaction Manager until the certified provider integration is
/// introduced in the cloud platform phase.
/// </summary>
public class InMemoryCloudTransactionClient : ICloudTransactionClient
{
    public Task<CloudTransactionSyncResult> SyncAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CloudTransactionSyncResult(Succeeded: true, Conflict: false));
    }
}
