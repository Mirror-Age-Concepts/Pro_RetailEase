# RetailEase

**Enterprise retail transaction and management platform.**

- **Primary language / stack:** C# / ASP.NET Core / .NET 9 LTS
- **Database:** Microsoft SQL Server (cloud); local-first storage for in-store (SQLite / SQL Server Express)
- **Architecture:** Distributed, modular, offline-capable (based on the NEXTEP architecture design breakdown)

> This is a production-grade retail platform for physical store operations, transaction processing,
> customer-facing interfaces, hardware peripherals, cloud services, licensing, synchronization and reporting.

## Current Implementation Status

| Phase | Scope | Status |
|---|---|---|
| 0 | Architecture, requirements validation, ADRs, backlog | In progress (artifacts created under `docs/`) |
| 1 | Engineering foundation | In progress — solution scaffold is green |
| 2 | In-store transaction engine | Pending |
| 3 | Client management and interfaces | Pending |
| 4 | Peripheral and external integrations | Pending |
| 5 | Cloud platform | Pending |
| 6 | Reporting and analytics | Pending |
| 7 | Security and resilience | Pending |
| 8 | Production release | Pending |

## Repository Layout

```
RetailEase.sln
src/
  RetailEase.Domain/
  RetailEase.Application/
  RetailEase.Infrastructure/
  RetailEase.Api/
tests/
  RetailEase.UnitTests/
  RetailEase.IntegrationTests/
database/
docs/
deployment/
scripts/
installers/
```

## Prerequisites

- .NET 9 SDK

## Build and Run

```bash
dotnet restore RetailEase.sln
dotnet build RetailEase.sln -c Debug
dotnet run --project src/RetailEase.Api
dotnet test RetailEase.sln -c Debug
```

## Phase 0 Deliverables

- Architecture Decision Records: `docs/adr/`
- Technical architecture: `docs/phase0/technical-architecture.md`
- Software requirements: `docs/phase0/software-requirements.md`
- Development backlog: `docs/phase0/development-backlog.md`
- Progress report: `docs/phase0/development-progress.md`

## Phase 1 Deliverables (completed / in progress)

- Solution and project scaffolding (`src/`, `tests/`).
- API host with `/health` self-check and OpenAPI in Development.
- Initial unit and integration test projects.

## Strict Development Rules (from master prompt)

1. No monolithic source file.
2. Core business logic stays C# / .NET.
3. No mock data as evidence of production functionality.
4. No hard-coded customer licensing.
5. No tenant isolation bypass.
6. Offline-only transactions are unacceptable.
7. No fake peripheral integrations.
8. No prohibited payment data in the non-PCI environment.
9. No silent error swallowing.
10. No undocumented architectural changes.

---

**Primary reference:** NEXTEP Architecture Design Breakdown.
**Target implementation:** RetailEase – C# / .NET Enterprise Retail Platform.
