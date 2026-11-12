using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;

namespace RetailEase.Application.Transactions;

/// <summary>
/// Application-level orchestration for the in-store transaction lifecycle:
/// create, queue for synchronization, acknowledge synchronization and retry failures.
/// </summary>
public interface ITransactionManager
{
    Task<Transaction> CreateAsync(CreateTransactionRequest request, CancellationToken cancellationToken = default);

    Task<Transaction> SubmitAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task<Transaction> MarkSyncedAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task<Transaction> MarkFailedAsync(Guid transactionId, string error, CancellationToken cancellationToken = default);
}
