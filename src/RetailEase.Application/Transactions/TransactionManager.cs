using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;

namespace RetailEase.Application.Transactions;

/// <summary>
/// Default in-process implementation of the transaction manager used by the in-store
/// Transaction Manager until a database-backed implementation is introduced.
/// </summary>
public class TransactionManager : ITransactionManager
{
    private readonly ITransactionStore _store;

    public TransactionManager(ITransactionStore store)
    {
        _store = store;
    }

    public async Task<Transaction> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default)
    {
        var transaction = Transaction.Create(
            request.StoreId,
            request.TerminalId,
            request.IdempotencyKey,
            request.CustomerId,
            request.Items);

        await _store.SaveAsync(transaction, cancellationToken);

        return transaction;
    }

    public async Task<Transaction> SubmitAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await GetRequiredAsync(transactionId, cancellationToken);

        transaction.Submit();

        await _store.SaveAsync(transaction, cancellationToken);

        return transaction;
    }

    public async Task<Transaction> MarkSyncedAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await GetRequiredAsync(transactionId, cancellationToken);

        transaction.MarkSynced();

        await _store.SaveAsync(transaction, cancellationToken);

        return transaction;
    }

    public async Task<Transaction> MarkFailedAsync(Guid transactionId, string error, CancellationToken cancellationToken = default)
    {
        var transaction = await GetRequiredAsync(transactionId, cancellationToken);

        transaction.MarkFailed(error);

        await _store.SaveAsync(transaction, cancellationToken);

        return transaction;
    }

    private async Task<Transaction> GetRequiredAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        var transaction = await _store.GetAsync(transactionId, cancellationToken);

        if (transaction is null)
        {
            throw new KeyNotFoundException($"Transaction with id '{transactionId}' was not found.");
        }

        return transaction;
    }
}
