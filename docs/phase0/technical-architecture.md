# Technical Architecture — RetailEase Phase 0

- **Status:** Accepted (Phase 0 baseline)
- **Date:** 2026-11-12

## 1. Purpose

Define the target technical architecture of RetailEase, the logical boundaries between modules, the data
strategy and the synchronization approach, before implementation begins.

## 2. Architectural Summary

RetailEase is decomposed into an **in-store runtime** and a **cloud service layer**, connected through a
synchronization plane.

### 2.1 In-Store Components

| Component | Responsibility |
|---|---|
| Transaction Manager (TM) | Central processing, local queue, offline-first, sync, reconciliation. |
| Client Manager | Device orchestration, UI launch, config/update distribution. |
| Client UIs | POS, Kiosk, DMD, OMS interfaces. |
| Peripheral Integration | Adapter framework for hardware. |

### 2.2 Cloud Components

| Component | Responsibility |
|---|---|
| Customer Portal | RBAC, store/device management, monitoring, reporting access. |
| Scheduler | Scheduled sync and jobs. |
| Licensing | Activation, validation, offline grace. |
| Customer Databases | Isolated per-customer databases. |
| Reporting | SQL/RDL-based business reports. |
| Software/Media | Versioned packages, checksums, signatures, distribution. |

### 2.3 API and Integration Layer

- REST APIs, webhooks, message queues or vendor SDKs for third-party integrations (payment gateways,
  loyalty, reservations, POS platforms).

## 3. Solution Structure

Target structure under `src/`, `tests/`, `database/`, `deployment/`, `docs/`, `scripts/`, `installers/`.

Current implementation state (Phase 1):

```
src/
  RetailEase.Domain/
  RetailEase.Application/
  RetailEase.Infrastructure/
  RetailEase.Api/
tests/
  RetailEase.UnitTests/
  RetailEase.IntegrationTests/
```

## 4. Data Architecture

- Cloud: Microsoft SQL Server (per-customer isolated databases).
- Local: SQLite or SQL Server Express (workload assessment pending).
- Migrations via EF Core tooling; referential integrity; indexes; audit columns; retention; tenant isolation;
  encryption at rest/in transit.

## 5. Synchronization Architecture

- Unique transaction identifiers, idempotency keys, checkpoints.
- Local-first queueing; retry on reconnect; reconciliation and conflict policies (to be defined per ADR-0003).

## 6. Security Architecture Summary

- RBAC server-side enforcement; MFA for privileged accounts; rate limiting; validation; audit logging.
- TLS in transit, encryption at rest; secrets management; device auth; signature verification.

## 7. Open Questions / Assumptions

See `docs/phase0/assumptions.md` (to be created) and Architecture Decision Records.
