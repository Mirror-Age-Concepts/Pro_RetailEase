namespace RetailEase.Domain.Entities;

/// <summary>
/// Lifecycle states of a retail transaction within the in-store Transaction Manager.
/// </summary>
public enum TransactionStatus
{
    /// <summary>Transaction created locally on the terminal; not yet queued for synchronization.</summary>
    Draft = 0,

    /// <summary>Transaction queued for cloud synchronization (or retry after failure).</summary>
    Submitted = 1,

    /// <summary>Transaction acknowledged by the cloud environment.</summary>
    Synced = 2,

    /// <summary>Transaction synchronization failed; pending retry.</summary>
    Failed = 3
}
