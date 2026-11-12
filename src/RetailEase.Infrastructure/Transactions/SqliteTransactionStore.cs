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

    public async Task SaveAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _database.Transactions.Add(transaction);
        await _database.SaveChangesAsync(cancellationToken);
    }

    public async Task<Transaction?> GetAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await _database.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(transaction => transaction.Id == transactionId, cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetPendingAsync(Guid storeId, Guid terminalId, CancellationToken cancellationToken = default)
    {
        var pendingTransactions = await _database.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.StoreId == storeId
                && transaction.TerminalId == terminalId
                && transaction.Status == TransactionStatus.Submitted)
            .ToListAsync(cancellationToken);

        return pendingTransactions;
    }

    public async Task<IReadOnlyList<Transaction>> GetFailedAsync(Guid storeId, Guid terminalId, CancellationToken cancellationToken = default)
    {
        var failedTransactions = await _database.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.StoreId == storeId
                && transaction.TerminalId == terminalId
                && transaction.Status == TransactionStatus.Failed)
            .ToListAsync(cancellationToken);

        return failedTransactions;
    }
}
