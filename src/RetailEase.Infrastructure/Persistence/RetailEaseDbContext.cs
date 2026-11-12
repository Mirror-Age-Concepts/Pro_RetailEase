using Microsoft.EntityFrameworkCore;

namespace RetailEase.Infrastructure.Persistence;

/// <summary>
/// Root Entity Framework Core DbContext for RetailEase.
/// Concrete aggregate DbSets will be introduced as modules are implemented
/// (see Phase 2 – In-Store Transaction Engine).
/// </summary>
public class RetailEaseDbContext : DbContext
{
    public RetailEaseDbContext(DbContextOptions<RetailEaseDbContext> options)
        : base(options)
    {
    }
}
