using RetailEase.Domain.Entities;

namespace RetailEase.Domain.Entities;

/// <summary>
/// Represents a retail transaction processed by the in-store Transaction Manager.
/// Maintains a strict, auditable lifecycle and supports offline-first queueing.
/// </summary>
public class Transaction
{
    public Guid Id { get; set; }

    public string IdempotencyKey { get; set; } = string.Empty;

    public Guid StoreId { get; set; }

    public Guid TerminalId { get; set; }

    public string? CustomerId { get; set; }

    public TransactionStatus Status { get; set; } = TransactionStatus.Draft;

    public string? LastError { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public DateTime? SyncedAtUtc { get; set; }

    public ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();

    /// <summary>
    /// Creates a new transaction in the Draft state. The idempotency key must be supplied by the client
    /// to guarantee safe, duplicate-free synchronization.
    /// </summary>
    public static Transaction Create(
        Guid storeId,
        Guid terminalId,
        string idempotencyKey,
        string? customerId = null,
        IReadOnlyCollection<TransactionItem>? items = null)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            throw new ArgumentException("A transaction idempotency key is required.", nameof(idempotencyKey));
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            StoreId = storeId,
            TerminalId = terminalId,
            CustomerId = customerId,
            Status = TransactionStatus.Draft,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        foreach (var item in items ?? Array.Empty<TransactionItem>())
        {
            transaction.Items.Add(item);
        }

        return transaction;
    }

    public void AddItem(TransactionItem item)
    {
        Items.Add(item);
    }

    /// <summary>Queues the transaction for cloud synchronization (Draft -> Submitted).</summary>
    public void Submit()
    {
        EnsureTransition(TransactionStatus.Draft, TransactionStatus.Submitted);

        Status = TransactionStatus.Submitted;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Records successful cloud synchronization (Submitted -> Synced).</summary>
    public void MarkSynced()
    {
        EnsureTransition(TransactionStatus.Submitted, TransactionStatus.Synced);

        Status = TransactionStatus.Synced;
        SyncedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Records a failed synchronization attempt (Submitted -> Failed).</summary>
    public void MarkFailed(string error)
    {
        EnsureTransition(TransactionStatus.Submitted, TransactionStatus.Failed);

        Status = TransactionStatus.Failed;
        LastError = error;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>Retries a failed synchronization (Failed -> Submitted).</summary>
    public void Retry()
    {
        EnsureTransition(TransactionStatus.Failed, TransactionStatus.Submitted);

        Status = TransactionStatus.Submitted;
        LastError = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private void EnsureTransition(TransactionStatus expectedStatus, TransactionStatus targetStatus)
    {
        if (Status != expectedStatus)
        {
            throw new InvalidOperationException(
                $"Invalid transaction state transition: expected '{expectedStatus}' but was '{Status}'.");
        }
    }
}
