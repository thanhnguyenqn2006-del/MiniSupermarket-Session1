README — Buổi 4 (JWT Refresh, Revoke, and Client Integration)

Goal
----
Document session 4: rotating refresh tokens, revoke endpoints, persistent storage (EF Core), WinForms client integration and testing guidance.

Key topics covered
------------------
- Refresh token rotation: refresh endpoint issues a new refresh token and invalidates the previous one.
- Refresh token persistence: store refresh tokens in database (RefreshTokens table) using EF Core.
- Revoke endpoints:
  - POST /api/Auth/revoke        — revoke a single refresh token (public)
  - POST /api/Auth/revoke-user   — Admin: revoke all tokens for a given user
  - POST /api/Auth/revoke-all    — Admin: revoke all refresh tokens (global)
- Security considerations: keep short lived access tokens (e.g., 15 min), rotate refresh tokens, validate token existence and expiry on refresh, store refresh tokens server-side.
- WinForms client integration:
  - SessionManager stores accessToken, refreshToken, and role.
  - ApiClient sends Bearer header and retries with refresh when 401 received.
  - Automatic re-login fallback if refresh fails.
- Custom validation and filters: ModelState response factory, ValidateModelAttribute, execution-time and logging filters.

Testing & quick commands
------------------------
- Login (get access + refresh tokens):
  curl -k -X POST "https://localhost:7006/api/Auth/login" -H "Content-Type: application/json" -d '{"username":"admin","password":"123456"}'

- Refresh (rotate):
  curl -k -X POST "https://localhost:7006/api/Auth/refresh" -H "Content-Type: application/json" -d '{"refreshToken":"<refreshToken>"}'

- Revoke single (public):
  curl -k -X POST "https://localhost:7006/api/Auth/revoke" -H "Content-Type: application/json" -d '{"refreshToken":"<refreshToken>"}'

- Revoke-user (Admin only):
  curl -k -X POST "https://localhost:7006/api/Auth/revoke-user" -H "Authorization: Bearer <accessToken>" -H "Content-Type: application/json" -d '{"username":"cashier"}'

Notes & troubleshooting
-----------------------
- Migrations: any change in seed or models requires `dotnet ef migrations add <Name>` and `dotnet ef database update`.
- If API is locked by a running process, stop it before running migrations or build.
- Verify the connection string in MiniSupermarket.API/appsettings.json to ensure the correct database is targeted.

Next improvements
-----------------
- Replace plaintext refresh tokens with hashed tokens in database.
- Move refresh token to HttpOnly cookie for browser clients if needed.
- Add unit/integration tests for AuthController and token lifecycle.


