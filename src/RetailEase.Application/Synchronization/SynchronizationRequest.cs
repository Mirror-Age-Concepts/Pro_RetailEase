namespace RetailEase.Application.Synchronization;

/// <summary>
/// Request to synchronize locally queued transactions to the cloud environment.
/// </summary>
public record SynchronizationRequest(
    Guid StoreId,
    Guid TerminalId);
