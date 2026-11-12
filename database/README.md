# Database

Database strategy for RetailEase.

## Cloud (Azure / production)

- Engine: Microsoft SQL Server.
- Layout: isolated per-customer databases (see prompt Section B4).
- Access: EF Core (`Microsoft.EntityFrameworkCore.SqlServer`) in `RetailEase.Infrastructure`.

## Local in-store

- Engine: SQLite or SQL Server Express, pending workload assessment (Phase 0/2).
- Local transaction storage for the Transaction Manager.

## Directory conventions

- `database/migrations/` — EF Core database migrations (to be introduced in Phase 2).
- `database/init/` — baseline scripts and seed data (where required).

Baseline SQL scripts and migration history will be added as the corresponding phases land.
