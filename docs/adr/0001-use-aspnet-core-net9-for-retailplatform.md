# ADR-0001 — Adopt ASP.NET Core on .NET 9 LTS as the RetailEase Backend Runtime

- **Status:** Accepted
- **Date:** 2026-11-12
- **Phase:** Phase 0 / Phase 1

## Context

RetailEase is an enterprise C# / .NET retail platform that must support high-volume in-store transaction
processing, offline-first local operation, a cloud-hosted multi-tenant service layer, hardware integration
and business reporting.

The architectural reference (`NEXTEP ARCHITECTURE DESIGN BREAKDOWN.pptm`) establishes the C# / ASP.NET Core
foundation.

## Decision

Adopt **ASP.NET Core on the .NET 9 LTS runtime** as the primary backend technology stack for RetailEase:

- ASP.NET Core Minimal APIs for service endpoints (evolving to controllers/REST as modules mature).
- Dependency injection, configuration and logging abstractions from the ASP.NET Core shared framework.
- EF Core for cloud database access; raw SQL or optimized queries where justified.

## Consequences

- **Benefits:** first-class enterprise ecosystem, strong async/performance characteristics, mature DI and
  observability support, single C# engineering standard.
- **Risks:** device/runtime footprint for offline-capable terminals; heavier hosting footprint than minimal
  runtimes (mitigated by modular boundaries and later per-module deployments).
- **Mitigations:** keep modules independently testable; use modular architecture with clear boundaries
  (Section 7 standards).

## Compliance

- Prompt Section 1 (Product identity), Section 7 (C# Software Engineering Standards).

---

# ADR-0002 — SQL Server as Primary Cloud Database with Local-First Embedded Database for In-Store

- **Status:** Accepted
- **Date:** 2026-11-12
- **Phase:** Phase 0 / Phase 2

## Context

Cloud architecture requires isolated per-customer databases on Microsoft SQL Server (Section 5, B4).
In-store devices must continue processing during internet outages and therefore need a local database
(Section 2, A1, A6).

## Decision

- **Cloud:** Microsoft SQL Server as the primary database engine for customer databases and shared services.
- **Local in-store:** SQLite or SQL Server Express, to be selected by workload assessment in Phase 0/2.

## Consequences

- **Benefits:** enterprise-grade cloud data platform; embedded, file-based local storage enabling offline-first.
- **Risks:** dual-engine consistency and synchronization complexity (mitigated by ADR-0003 and sync protocol).
- **Mitigations:** explicit synchronization conflict policies and reconciliation procedures.

## Compliance

- Prompt Sections 2, 5, 6.

---

# ADR-0003 — Local-First Offline Transaction Engine with Idempotent Synchronization

- **Status:** Accepted
- **Date:** 2026-11-12
- **Phase:** Phase 0 / Phase 2

## Context

The Transaction Manager must remain operational during temporary internet outages and must safely queue,
retry and synchronize transactions (Section 2, A1, A6).

## Decision

Implement a **local-first transaction processing mechanism**:

- Local transaction storage with XML and JSON serialization and schema validation (A6).
- Idempotency keys and synchronization checkpoints to prevent duplicate processing (A1).
- Automatic retry and failed-synchronization recovery after connectivity returns (A1, A6).

## Consequences

- **Benefits:** continuity of sales operations during outages; auditability of queued transactions.
- **Risks:** local storage corruption (mitigated by protected queue and integrity handling).
- **Mitigations:** structured failure handling, reconciliation, recovery tooling.

## Compliance

- Prompt Sections 2, 5, 6, 13.

---

# ADR-0004 — Strict Separation of Non-PCI and Payment-Sensitive Environments

- **Status:** Accepted
- **Date:** 2026-11-12
- **Phase:** Phase 0 / Phase 4

## Context

The Azure Non-PCI cloud environment must not contain raw card numbers, CVV or PIN data (Section 4).

## Decision

Keep the RetailEase non-PCI environment free of prohibited payment authentication data; store only payment
references, transaction status, approved amounts and permitted reconciliation metadata; route actual payment
processing through certified providers and terminals.

## Consequences

- **Benefits:** reduced compliance surface and safer data handling posture.
- **Risks:** integration complexity with certified payment providers.
- **Mitigations:** certified integrations, independent PCI DSS scoping and compliance assessment.

## Compliance

- Prompt Section 4, 13.

---

# ADR-0005 — Modular Solution Layout with Independent Deployable Components

- **Status:** Proposed
- **Date:** 2026-11-12
- **Phase:** Phase 1 / Phase 5

## Context

The required structure calls for separate projects for Transaction Manager, Client Manager, Synchronization,
API, Cloud Services, Licensing, Scheduler, Reporting, Customer Portal and device UIs.

## Decision

Adopt the prescribed modular layout under `src/` and `tests/` with clear dependency direction:

- `RetailEase.Domain` — core entities and business rules.
- `RetailEase.Application` — use cases and orchestration contracts.
- `RetailEase.Infrastructure` — implementations (DB, hardware, sync).
- `RetailEase.Api`, `RetailEase.CloudServices`, etc. — host-specific composition.

## Consequences

- **Benefits:** independent testability and deployment.
- **Risks:** up-front structuring effort; mitigated by avoiding over-abstraction (Section 7).

## Compliance

- Prompt Section 10, 7.
