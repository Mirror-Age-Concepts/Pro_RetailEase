using RetailEase.Application.Synchronization;
using RetailEase.Application.Transactions;
using RetailEase.Domain.Entities;
using RetailEase.Infrastructure.Synchronization;
using RetailEase.Infrastructure.Transactions;

namespace RetailEase.UnitTests.Transactions;

public class TransactionSynchronizerTests
{
    [Fact]
    public async Task SynchronizeAsync_WithoutPendingTransactions_ReturnsEmptyResult()
    {
        var store = new InMemoryTransactionStore();
        var client = new FakeCloudTransactionClient(success: true);
        var sut = new TransactionSynchronizer(store, client);

        var result = await sut.SynchronizeAsync(new SynchronizationRequest(StoreId: Guid.NewGuid(), TerminalId: Guid.NewGuid()));

        Assert.Equal(0, result.ProcessedCount);
        Assert.Equal(0, result.SucceededCount);
        Assert.Equal(0, result.FailedCount);
        Assert.Null(result.LastCheckpointUtc);
    }

    [Fact]
    public async Task SynchronizeAsync_MarksPendingTransactionSyncedAfterSuccessfulCloudSync()
    {
        var store = new InMemoryTransactionStore();
        var manager = new TransactionManager(store);
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();

        var created = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: storeId,
            TerminalId: terminalId,
            IdempotencyKey: "sync-001",
            CustomerId: null,
            Items: Array.Empty<TransactionItem>()));

        await manager.SubmitAsync(created.Id);

        var client = new FakeCloudTransactionClient(success: true);
        var sut = new TransactionSynchronizer(store, client);

        var result = await sut.SynchronizeAsync(new SynchronizationRequest(storeId, terminalId));

        Assert.Equal(1, result.ProcessedCount);
        Assert.Equal(1, result.SucceededCount);
        Assert.Equal(0, result.FailedCount);
        Assert.NotNull(result.LastCheckpointUtc);

        var stored = await store.GetAsync(created.Id);
        Assert.Equal(TransactionStatus.Synced, stored!.Status);
    }

    [Fact]
    public async Task SynchronizeAsync_MarksPendingTransactionFailedAfterUnsuccessfulCloudSync()
    {
        var store = new InMemoryTransactionStore();
        var manager = new TransactionManager(store);
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();

        var created = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: storeId,
            TerminalId: terminalId,
            IdempotencyKey: "sync-002",
            CustomerId: null,
            Items: Array.Empty<TransactionItem>()));

        await manager.SubmitAsync(created.Id);

        var client = new FakeCloudTransactionClient(success: false, errorMessage: "cloud-unavailable");
        var sut = new TransactionSynchronizer(store, client);

        var result = await sut.SynchronizeAsync(new SynchronizationRequest(storeId, terminalId));

        Assert.Equal(1, result.ProcessedCount);
        Assert.Equal(0, result.SucceededCount);
        Assert.Equal(1, result.FailedCount);

        var stored = await store.GetAsync(created.Id);
        Assert.Equal(TransactionStatus.Failed, stored!.Status);
        Assert.Equal("cloud-unavailable", stored.LastError);
    }

    [Fact]
    public async Task SynchronizeAsync_SecondPassAfterSuccessfulSyncProcessesNoFurtherTransactions()
    {
        var store = new InMemoryTransactionStore();
        var manager = new TransactionManager(store);
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();

        var created = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: storeId,
            TerminalId: terminalId,
            IdempotencyKey: "sync-003",
            CustomerId: null,
            Items: Array.Empty<TransactionItem>()));
        await manager.SubmitAsync(created.Id);

        var client = new FakeCloudTransactionClient(success: true);
        var sut = new TransactionSynchronizer(store, client);

        var firstResult = await sut.SynchronizeAsync(new SynchronizationRequest(storeId, terminalId));
        var secondResult = await sut.SynchronizeAsync(new SynchronizationRequest(storeId, terminalId));

        Assert.Equal(1, firstResult.ProcessedCount);
        Assert.Equal(0, secondResult.ProcessedCount);
    }

    [Fact]
    public async Task RetryFailedAsync_RetriesFailedTransactionAndSucceedsOnReconnection()
    {
        var store = new InMemoryTransactionStore();
        var manager = new TransactionManager(store);
        var storeId = Guid.NewGuid();
        var terminalId = Guid.NewGuid();

        var created = await manager.CreateAsync(new CreateTransactionRequest(
            StoreId: storeId,
            TerminalId: terminalId,
            IdempotencyKey: "sync-004",
            CustomerId: null,
            Items: Array.Empty<TransactionItem>()));

        await manager.SubmitAsync(created.Id);

        var failingClient = new FakeCloudTransactionClient(success: false, errorMessage: "cloud-unavailable");
        var failingSut = new TransactionSynchronizer(store, failingClient);
        var failedResult = await failingSut.SynchronizeAsync(new SynchronizationRequest(storeId, terminalId));
        Assert.Equal(1, failedResult.FailedCount);

        var succeedingClient = new FakeCloudTransactionClient(success: true);
        var retrySut = new TransactionSynchronizer(store, succeedingClient);
        var retryResult = await retrySut.RetryFailedAsync(new SynchronizationRequest(storeId, terminalId));

        Assert.Equal(1, retryResult.ProcessedCount);
        Assert.Equal(1, retryResult.SucceededCount);

        var stored = await store.GetAsync(created.Id);
        Assert.Equal(TransactionStatus.Synced, stored!.Status);
    }

    [Fact]
    public async Task RetryFailedAsync_WithoutFailedTransactions_ReturnsEmptyResult()
    {
        var store = new InMemoryTransactionStore();
        var client = new FakeCloudTransactionClient(success: true);
        var sut = new TransactionSynchronizer(store, client);

        var result = await sut.RetryFailedAsync(new SynchronizationRequest(StoreId: Guid.NewGuid(), TerminalId: Guid.NewGuid()));

        Assert.Equal(0, result.ProcessedCount);
        Assert.Equal(0, result.SucceededCount);
        Assert.Equal(0, result.FailedCount);
    }
}

/// <summary>
/// Test double implementing the cloud transaction client contract.
/// </summary>
internal sealed class FakeCloudTransactionClient : ICloudTransactionClient
{
    private readonly bool _success;
    private readonly string? _errorMessage;

    public FakeCloudTransactionClient(bool success, string? errorMessage = null)
    {
        _success = success;
        _errorMessage = errorMessage;
    }

    public Task<CloudTransactionSyncResult> SyncAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new CloudTransactionSyncResult(
            Succeeded: _success,
            ErrorCode: _success ? null : "CloudUnavailable",
            ErrorMessage: _success ? null : _errorMessage));
    }
}
