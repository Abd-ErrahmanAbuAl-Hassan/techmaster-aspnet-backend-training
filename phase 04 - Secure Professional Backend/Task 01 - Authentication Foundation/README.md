# 🔐 Phase 04 — Task 01: Authentication Foundation

The identity layer for the Training Center API: real users can **register**, **log in**, and receive a **JWT access token** (plus a rotating refresh token). Students, instructors, and admins no longer reach the API anonymously — each user has credentials, a role, an active status, and a hashed password.

---

## 📖 Table of Contents

- [Endpoints](#-endpoints)
- [Register Flow](#-register-flow)
- [Login Flow](#-login-flow)
- [JWT Flow](#-jwt-flow)
- [Rules & Where They're Enforced](#-rules--where-theyre-enforced)
- [Reproducing the Invalid Cases](#-reproducing-the-invalid-cases)
- [Evidence / Screenshots](#-evidence--screenshots)

---

## 📡 Endpoints

| Method | Endpoint | Auth | Spec | Description |
|--------|----------|------|------|-------------|
| POST | `/api/auth/register` | Anonymous | Required | Create a Student or Instructor account (Admin is blocked) |
| POST | `/api/auth/login` | Anonymous | Required | Verify credentials, return access token + refresh token |
| GET | `/api/auth/me` | **Bearer** | Required | Return the current user's identity summary |
| POST | `/api/auth/change-password` | **Bearer** | Optional | Change password (requires the old password) |
| POST | `/api/auth/refresh-token` | Anonymous | Advanced | Exchange a refresh token for a new access + refresh token pair |
| POST | `/api/auth/logout` | **Bearer** | Advanced | Revoke a refresh token |

---

## 📝 Register Flow

`POST /api/auth/register` → `AuthController.Register` → `AuthService.RegisterAsync`

| Step | What happens | Where |
|------|--------------|-------|
| 1 | Receive `RegisterRequest` (name, email, phone, password, confirm password, role) | Controller model binding + `ModelState` |
| 2 | Validate email format, password strength, password = confirm password | `AuthValidation.RegisterValidate` |
| 3 | **Block `Admin`** — public registration is limited to Student/Instructor | `AuthController.Register` → `400` |
| 4 | Reject a duplicate email | `AuthService.RegisterAsync` → `409` |
| 5 | Hash the password — the plain text is never stored | `IPasswordHasher.Hash` |
| 6 | Create the right user type for the role, `IsActive = true` | `Role.Student → Student`, `Role.Instructor → Instructor` |
| 7 | Return an identity summary — **never** the hash | `CurrentUserResponse` → `201 Created` |

Example success response:

```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "userId": 7,
    "fullName": "Mohamed Ayman",
    "email": "mohamed@example.com",
    "role": "Student"
  }
}
```

---

## 🔑 Login Flow

`POST /api/auth/login` → `AuthController.Login` → `AuthService.LoginAsync`

| Step | What happens | Failure response |
|------|--------------|------------------|
| 1 | Receive `LoginRequest`; email and password are required | `400` |
| 2 | Find the user by email | `401` *"Invalid credentials."* |
| 3 | Reject inactive users | `409` *"Login Failed. — User is inactive"* |
| 4 | Verify the password against the stored hash | `401` *"Invalid credentials."* |
| 5 | Generate the JWT access token | `500` if no token could be produced |
| 6 | Generate a refresh token; store **only its hash** in `RefreshTokens` | — |
| 7 | Update `LastLoginAt` | — |
| 8 | Return `AuthResponse` (user id, name, email, role, token, refresh token, expiry) | — |

Wrong email and wrong password deliberately return the **same** `401 Invalid credentials.` message, so the endpoint can't be used to discover which emails exist.

---

## 🎟 JWT Flow

1. **Login** — the client sends email + password and gets back an **access token** (short-lived JWT) and a **refresh token** (long-lived, random).
2. **Calling the API** — the client sends `Authorization: Bearer <access token>`. Endpoints marked `[Authorize]` reject requests with no token, an expired token, or an invalid signature with `401`.
3. **Identifying the caller** — `[Authorize]` actions read the user id from the token's `NameIdentifier` claim (`User.FindFirstValue(ClaimTypes.NameIdentifier)`); the client never sends its own user id.
4. **Access token expires** — the client calls `POST /api/auth/refresh-token` with its refresh token.
5. **Refresh-token rotation** — the old refresh token is marked revoked (`RevokedAt`) and a brand-new access + refresh pair is issued, so each refresh token is single-use.
6. **Logout** — `POST /api/auth/logout` revokes the refresh token, so it can no longer be exchanged for new access tokens.

**Refresh tokens are stored hashed** (`ITokenGenerator.HashToken`), the same way passwords are — a database leak doesn't expose usable tokens.

**What belongs in the token** (per the spec): user id (`sub`/`nameidentifier`), `email`, `role`, optional `jti`, and `exp`. **What must never be in it:** password, password hash, connection strings, private notes, full payment details, secrets or API keys. Decode a real token at [jwt.io](https://jwt.io) and confirm this before submitting — see [Evidence](#-evidence--screenshots).

---

## ✅ Rules & Where They're Enforced

| Rule (from the spec) | Enforced in |
|----------------------|-------------|
| Email must be unique | `AuthService.RegisterAsync` (checked via `Users.GetByEmailAsync`) |
| Password cannot be stored as plain text | `IPasswordHasher.Hash` on register and change-password; `Verify` on login |
| Inactive users cannot log in | `AuthService.LoginAsync` (and `GetCurrentUserAsync`, `RefreshTokenAsync`) |
| Register role must be controlled — no self-registering as Admin | `AuthController.Register` → `400` |
| Login returns a safe response, not the full user entity | `AuthResponse` / `CurrentUserResponse` DTOs |

---

## 🧪 Reproducing the Invalid Cases

```http
# Wrong email -> 401 "Invalid credentials."
POST /api/auth/login
{ "email": "nobody@example.com", "password": "Valid#Pass123" }

# Wrong password -> 401 "Invalid credentials."
POST /api/auth/login
{ "email": "mohamed@example.com", "password": "WrongPassword1!" }

# Inactive user -> 409 "Login Failed." / "User is inactive"
POST /api/auth/login
{ "email": "inactive.user@example.com", "password": "Valid#Pass123" }

# Empty password -> 400 validation error
POST /api/auth/login
{ "email": "mohamed@example.com", "password": "" }

# Duplicate registered email -> 409 "Email was taken by another user."
POST /api/auth/register
{ "firstName": "Mohamed", "lastName": "Ayman", "email": "mohamed@example.com",
  "phoneNumber": "01012345678", "password": "Valid#Pass123",
  "confirmPassword": "Valid#Pass123", "role": "Student" }

# Registering as Admin -> 400 "You can only register as student or instructor."
POST /api/auth/register
{ ...same body..., "role": "Admin" }

# No token -> 401
GET /api/auth/me

# Valid token -> 200 with the current user
GET /api/auth/me
Authorization: Bearer <access token>
```

Set the inactive user up by flipping `IsActive` to `0` on a test row in the database.

---



## 🖼 Evidence / Screenshots

> 📌 **[View Evidence on Google Drive](https://drive.google.com/drive/folders/18WdntiKi06a-Yhq81iB7BIaNFpHwUM9w?usp=drive_link)**
