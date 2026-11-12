using Microsoft.EntityFrameworkCore;
using RetailEase.Domain.Entities;

namespace RetailEase.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the local in-store SQLite transaction database.
/// </summary>
public class SqliteDbContext : DbContext
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");
            entity.HasKey(transaction => transaction.Id);
            entity.Property(transaction => transaction.IdempotencyKey).HasMaxLength(128).IsRequired();
            entity.HasIndex(transaction => transaction.IdempotencyKey).IsUnique();
            entity.Property(transaction => transaction.StoreId).IsRequired();
            entity.Property(transaction => transaction.TerminalId).IsRequired();
            entity.Property(transaction => transaction.CustomerId).HasMaxLength(128);
            entity.Property(transaction => transaction.Status).HasConversion<int>();
            entity.Property(transaction => transaction.LastError).HasMaxLength(512);
            entity.Property(transaction => transaction.CreatedAtUtc).HasColumnType("datetime");
            entity.Property(transaction => transaction.UpdatedAtUtc).HasColumnType("datetime");
            entity.Property(transaction => transaction.SyncedAtUtc).HasColumnType("datetime");

            entity.HasMany(transaction => transaction.Items)
                .WithOne(item => item.Transaction)
                .HasForeignKey(item => item.TransactionId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
