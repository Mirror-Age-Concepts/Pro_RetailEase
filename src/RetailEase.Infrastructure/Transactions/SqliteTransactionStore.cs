using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RetailEase.Application.Stores;
using RetailEase.Domain.Entities;
using RetailEase.Infrastructure.Persistence;

namespace RetailEase.Infrastructure.Transactions;

/// <summary>
/// SQLite-backed implementation of <see cref="ITransactionStore"/> for the local
/// in-store transaction database.
/// </summary>
public class SqliteTransactionStore : ITransactionStore
{
    private readonly SqliteDbContext _database;

    public SqliteTransactionStore(SqliteDbContext database)
    {
        _database = database;
    }

    public Task SaveAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _database.Transactions.Add(transaction);
        _database.SaveChanges(cancellationToken);

        return Task.CompletedTask;
    }

    public async Task<Transaction?> GetAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _database.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(transaction => transaction.Id == transactionId, cancellationToken);
    }
}
