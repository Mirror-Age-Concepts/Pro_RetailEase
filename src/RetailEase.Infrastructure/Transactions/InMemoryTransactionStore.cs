using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;

namespace RetailEase.Infrastructure.Transactions;

/// <summary>
/// In-memory implementation of <see cref="ITransactionStore"/> used as the local
/// transaction persistence backend while the SQLite-backed store is introduced in Phase 2.
/// </summary>
public class InMemoryTransactionStore : ITransactionStore
{
    private readonly Dictionary<Guid, Transaction> _transactions = new();

    private readonly Dictionary<string, Guid> _idempotencyKeys = new(StringComparer.OrdinalIgnoreCase);

    public Task SaveAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        if (_idempotencyKeys.TryGetValue(transaction.IdempotencyKey, out var existingTransactionId)
            && existingTransactionId != transaction.Id)
        {
            throw new InvalidOperationException(
                $"Duplicate transaction idempotency key '{transaction.IdempotencyKey}' rejected.");
        }

        _idempotencyKeys[transaction.IdempotencyKey] = transaction.Id;
        _transactions[transaction.Id] = transaction;

        return Task.CompletedTask;
    }

    public Task<Transaction?> GetAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_transactions.TryGetValue(transactionId, out var transaction) ? transaction : null);
    }

    public Task<IReadOnlyList<Transaction>> GetPendingAsync(Guid storeId, Guid terminalId, CancellationToken cancellationToken = default)
    {
        var pendingTransactions = _transactions.Values
            .Where(transaction => transaction.StoreId == storeId
                && transaction.TerminalId == terminalId
                && transaction.Status == TransactionStatus.Submitted)
            .ToList();

        return Task.FromResult<IReadOnlyList<Transaction>>(pendingTransactions);
    }

    public Task<IReadOnlyList<Transaction>> GetFailedAsync(Guid storeId, Guid terminalId, CancellationToken cancellationToken = default)
    {
        var failedTransactions = _transactions.Values
            .Where(transaction => transaction.StoreId == storeId
                && transaction.TerminalId == terminalId
                && transaction.Status == TransactionStatus.Failed)
            .ToList();

        return Task.FromResult<IReadOnlyList<Transaction>>(failedTransactions);
    }
}
