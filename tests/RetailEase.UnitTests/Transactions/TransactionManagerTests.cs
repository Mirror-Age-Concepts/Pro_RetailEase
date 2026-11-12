using RetailEase.Application.Transactions;
using RetailEase.Domain.Entities;
using RetailEase.Infrastructure.Transactions;

namespace RetailEase.UnitTests.Transactions;

public class TransactionManagerTests
{
    private readonly InMemoryTransactionStore _store = new();

    [Fact]
    public async Task CreateAsync_PersistsDraftTransaction()
    {
        var manager = new TransactionManager(_store);

        var transaction = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: Guid.NewGuid(),
            TerminalId: Guid.NewGuid(),
            IdempotencyKey: "txn-manager-001",
            CustomerId: "customer-9",
            Items: Array.Empty<TransactionItem>()));

        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(TransactionStatus.Draft, transaction.Status);
    }

    [Fact]
    public async Task SubmitAsync_TransitionsDraftTransactionToSubmitted()
    {
        var manager = new TransactionManager(_store);

        var created = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: Guid.NewGuid(),
            TerminalId: Guid.NewGuid(),
            IdempotencyKey: "txn-manager-002",
            CustomerId: null,
            Items: Array.Empty<TransactionItem>()));

        var submitted = await manager.SubmitAsync(created.Id);

        Assert.Equal(TransactionStatus.Submitted, submitted.Status);
    }

    [Fact]
    public async Task SubmitAsync_UnknownTransaction_ThrowsKeyNotFound()
    {
        var manager = new TransactionManager(_store);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => manager.SubmitAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task MarkSyncedAsync_TransitionsSubmittedTransactionToSynced()
    {
        var manager = new TransactionManager(_store);

        var created = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: Guid.NewGuid(),
            TerminalId: Guid.NewGuid(),
            IdempotencyKey: "txn-manager-003",
            CustomerId: null,
            Items: Array.Empty<TransactionItem>()));

        await manager.SubmitAsync(created.Id);
        var synced = await manager.MarkSyncedAsync(created.Id);

        Assert.Equal(TransactionStatus.Synced, synced.Status);
    }
}
