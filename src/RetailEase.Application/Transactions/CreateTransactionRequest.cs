namespace RetailEase.Application.Transactions;

/// <summary>
/// Command to create a new retail transaction for processing.
/// </summary>
public record CreateTransactionRequest(
    Guid StoreId,
    Guid TerminalId,
    string IdempotencyKey,
    string? CustomerId,
    IReadOnlyCollection<Domain.Entities.TransactionItem> Items);
