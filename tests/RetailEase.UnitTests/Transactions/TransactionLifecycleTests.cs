using RetailEase.Domain.Entities;

namespace RetailEase.UnitTests.Transactions;

public class TransactionLifecycleTests
{
    [Fact]
    public void Create_ProducesDraftTransaction()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-001",
            customerId: "customer-1");

        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(TransactionStatus.Draft, transaction.Status);
        Assert.Equal("txn-001", transaction.IdempotencyKey);
        Assert.NotEqual(Guid.Empty, transaction.StoreId);
    }

    [Fact]
    public void Submit_TransitionsDraftToSubmitted()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-002");

        transaction.Submit();

        Assert.Equal(TransactionStatus.Submitted, transaction.Status);
    }

    [Fact]
    public void MarkSynced_TransitionsSubmittedToSynced()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-003");

        transaction.Submit();
        transaction.MarkSynced();

        Assert.Equal(TransactionStatus.Synced, transaction.Status);
        Assert.NotNull(transaction.SyncedAtUtc);
    }

    [Fact]
    public void MarkFailed_TransitionsSubmittedToFailed()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-004");

        transaction.Submit();
        transaction.MarkFailed("cloud-unavailable");

        Assert.Equal(TransactionStatus.Failed, transaction.Status);
        Assert.Equal("cloud-unavailable", transaction.LastError);
    }

    [Fact]
    public void Retry_TransitionsFailedToSubmitted()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-005");

        transaction.Submit();
        transaction.MarkFailed("cloud-unavailable");
        transaction.Retry();

        Assert.Equal(TransactionStatus.Submitted, transaction.Status);
        Assert.Null(transaction.LastError);
    }

    [Fact]
    public void InvalidTransitionFromSyncedToSubmitted_Throws()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-006");

        transaction.Submit();
        transaction.MarkSynced();

        Assert.Throws<InvalidOperationException>(() => transaction.Submit());
    }

    [Fact]
    public void InvalidTransitionFromDraftToSynced_Throws()
    {
        var transaction = Transaction.Create(
            storeId: Guid.NewGuid(),
            terminalId: Guid.NewGuid(),
            idempotencyKey: "txn-007");

        Assert.Throws<InvalidOperationException>(() => transaction.MarkSynced());
    }
}
