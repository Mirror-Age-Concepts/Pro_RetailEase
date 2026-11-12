namespace RetailEase.Application.Synchronization;

/// <summary>
/// Outcome of a synchronization pass over locally queued transactions.
/// </summary>
public record SyncResult(
    int ProcessedCount,
    int SucceededCount,
    int FailedCount,
    DateTime? LastCheckpointUtc,
    int ConflictsCount);
