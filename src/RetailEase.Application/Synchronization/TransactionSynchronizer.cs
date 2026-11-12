using RetailEase.Application.Synchronization;
using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;

namespace RetailEase.Application.Synchronization;

/// <summary>
/// Orchestrates synchronization of locally queued transactions (outbox) to the
/// cloud environment, applying the transaction lifecycle and persisting outcomes.
/// </summary>
public class TransactionSynchronizer : ISynchronizationEngine
{
    private readonly ITransactionStore _store;
    private readonly ICloudTransactionClient _cloudClient;

    public TransactionSynchronizer(ITransactionStore store, ICloudTransactionClient cloudClient)
    {
        _store = store;
        _cloudClient = cloudClient;
    }

    public async Task<SyncResult> SynchronizeAsync(SynchronizationRequest request, CancellationToken cancellationToken = default)
    {
        var pendingTransactions = await _store.GetPendingAsync(request.StoreId, request.TerminalId, cancellationToken);

        var succeededCount = 0;
        var failedCount = 0;
        DateTime? lastCheckpointUtc = null;

        foreach (var transaction in pendingTransactions)
        {
            var cloudResult = await _cloudClient.SyncAsync(transaction, cancellationToken);

            if (cloudResult.Succeeded)
            {
                transaction.MarkSynced();
                await _store.SaveAsync(transaction, cancellationToken);
                succeededCount++;
                lastCheckpointUtc = transaction.SyncedAtUtc;
            }
            else
            {
                transaction.MarkFailed(cloudResult.ErrorMessage ?? "Cloud transaction synchronization failed.");
                await _store.SaveAsync(transaction, cancellationToken);
                failedCount++;
            }
        }

        return new SyncResult(
            ProcessedCount: pendingTransactions.Count,
            SucceededCount: succeededCount,
            FailedCount: failedCount,
            LastCheckpointUtc: lastCheckpointUtc);
    }

    public async Task<SyncResult> RetryFailedAsync(SynchronizationRequest request, CancellationToken cancellationToken = default)
    {
        var failedTransactions = await _store.GetFailedAsync(request.StoreId, request.TerminalId, cancellationToken);

        foreach (var transaction in failedTransactions)
        {
            transaction.Retry();
            await _store.SaveAsync(transaction, cancellationToken);
        }

        return await SynchronizeAsync(request, cancellationToken);
    }
}
