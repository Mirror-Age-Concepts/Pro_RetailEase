using RetailEase.Domain.Entities;

namespace RetailEase.Application.Synchronization;

/// <summary>
/// Contract for the outbound cloud transaction synchronization client.
/// Implemented by the certified cloud integration in later phases.
/// </summary>
public interface ICloudTransactionClient
{
    Task<CloudTransactionSyncResult> SyncAsync(Transaction transaction, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a single cloud transaction synchronization attempt.
/// </summary>
public record CloudTransactionSyncResult(
    bool Succeeded,
    string? ErrorCode = null,
    string? ErrorMessage = null);
