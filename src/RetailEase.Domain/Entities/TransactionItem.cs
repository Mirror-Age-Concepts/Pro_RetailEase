namespace RetailEase.Domain.Entities;

/// <summary>
/// A single line item within a retail transaction.
/// </summary>
public class TransactionItem
{
    public Guid Id { get; set; }

    public Guid TransactionId { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public Transaction? Transaction { get; set; }
}
