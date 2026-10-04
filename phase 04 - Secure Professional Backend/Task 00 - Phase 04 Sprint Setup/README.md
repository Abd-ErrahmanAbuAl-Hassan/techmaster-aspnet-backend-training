# Phase 04 - Secure Professional Backend

## Baseline
- Phase 03 API: Training Center Registration API
- Database: SQL Server / Remote DB
- Deployment: Not yet deployed (local/dev only)

## Phase 03 Limitations
Known gaps and open issues in the Phase 03 baseline, carried over as the starting point for this sprint rather than silently left undocumented:

- No authentication — every endpoint (Students, Instructors, Tracks, Enrollments, Payments, Reports) is fully open with no identity check at all.
- No authorization / role-based access control — e.g. a track update/delete only checks an `instructorId` passed as a plain query parameter, which anyone can spoof; there's no real concept of Admin, Instructor, or Student yet.
- No global exception-handling middleware — each service hand-rolls its own try/catch, and a few paths return a raw `ex.Message` (or even `ex.InnerException.Message`) straight to the client in a `500` response instead of a clean, generic error.
- No audit trail of *who* performed an action — `CreatedAt`/`UpdatedAt` timestamps exist, but there's no actor/user captured anywhere, so changes can't be attributed to a person.
- Revenue and payment-status calculations have needed repeated point-fixes (filtering to `Paid`/`PartiallyPaid` only) across multiple services — a sign this logic should be centralized rather than recalculated ad hoc wherever it's needed.
- No rate limiting or request throttling on any endpoint.

## Phase 04 Goals
- Add authentication and JWT
- Add Admin / Instructor / Student roles
- Protect endpoints
- Add global error handling and logging
- Add audit trail
- Redeploy live API
- Publish LinkedIn showcase

## Backlog Status
| Item | Status | Notes |
|------|--------|-------|
| Auth foundation (JWT issuance, login endpoint) | Not Started | |
| Role rules (Admin / Instructor / Student) | Not Started | Replace spoofable `instructorId` query param with the authenticated user's identity |
| Endpoint protection (`[Authorize]` + policy per role) | Not Started | |
| Global error-handling middleware | Not Started | Replace per-service try/catch leaking `ex.Message` with a single exception-handling pipeline |
| Audit trail (who did what, when) | Not Started | |
| Centralize revenue/payment-status calculation | Not Started | |
| Deployment | Not Started | |
| LinkedIn showcase post | Not Started | |
