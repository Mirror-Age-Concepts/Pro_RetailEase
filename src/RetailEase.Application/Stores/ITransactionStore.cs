using RetailEase.Domain.Entities;

namespace RetailEase.Application.Stores;

/// <summary>
/// Persistence abstraction for local (in-store) transaction storage.
/// Implementations will target the local database (SQLite / SQL Server Express)
/// and later the cloud customer database during synchronization.
/// </summary>
public interface ITransactionStore
{
    Task SaveAsync(Transaction transaction, CancellationToken cancellationToken = default);

    Task<Transaction?> GetAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
