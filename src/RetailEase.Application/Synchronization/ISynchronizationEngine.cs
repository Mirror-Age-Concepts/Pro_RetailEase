using RetailEase.Application.Synchronization;

namespace RetailEase.Application.Synchronization;

/// <summary>
/// Engine responsible for synchronizing locally queued retail transactions to the cloud.
/// </summary>
public interface ISynchronizationEngine
{
    Task<SyncResult> SynchronizeAsync(SynchronizationRequest request, CancellationToken cancellationToken = default);

    Task<SyncResult> RetryFailedAsync(SynchronizationRequest request, CancellationToken cancellationToken = default);
}
