# Medieval Chess - Security Model

## 1. Authentication & Identity
Medieval Chess utilizes a robust tiered authentication system to ensure player identity and account security.

### 1.1 Identity Provider
- **System:** ASP.NET Core Identity
- **External Auth:** OAuth 2.0 via Google and GitHub.
- **Tokens:**
  - **Access Token:** JWT (15 min expiry), contains `sub` (UserId), `role`, and `game_id` claims.
  - **Refresh Token:** High-entropy random string (7 day sliding window), stored in HttpOnly, Secure cookies.

### 1.2 Session Management
- **Concurrent Sessions:** Allowed, but active game moves validated against specific session keys if enabled.
- **Force Logout:** Admin can revoke refresh tokens to terminate sessions immediately.

---

## 2. Gameplay Integrity (Anti-Cheat)

### 2.1 Authoritative Server
**Rule #1:** The Client is never trusted.
- All game logic (movement, combat resolution, loyalty calculation) is executed on the server.
- The client is merely a specialized renderer and input device.
- "Teleport hacks" or "Stat editing" are impossible as the server state is the single source of truth.

### 2.2 Cryptographic Move Signing and Auditing
To prevent "Man-in-the-Middle" attacks or repudiation (players claiming they didn't make a move):
1. **Key Generation:** On account creation, a client-side RSA-2048 key pair is generated.
2. **Signing:** Every move payload is signed with the user's Private Key.
3. **Verification:** Server validates signature with stored Public Key before processing.
4. **Audit Chain:**
   - Each move includes the Hash of the previous move.
   - `Hash(N) = SHA256(Hash(N-1) + MoveData + Timestamp + Signature)`
   - This creates an immutable blockchain-like ledger for every match, allowing perfect replay verification.

### 2.3 Algorithmic Analysis
- **Timing Analysis:** Detecting bot-like consistency (e.g., always moving in exactly 150ms).
- **Impossible Sequences:** Flagging moves that would require client-side state manipulation (though rejected by server, the *attempt* is logged).

---

## 3. Data Privacy
- **Email Addresses:** Never exposed to opponents.
- **Chat:** Text is sanitized for XSS and profanity stored encrypted at rest.
- **GDPR Compliance:** "Right to be Forgotten" endpoint performs soft-delete on PII, anonymizing game history.

---

## 4. Infrastructure Security
- **Rate Limiting:** IP-based rate limiting (100 req/min) on API endpoints to prevent DDoS.
- **Input Sanitization:** All text inputs (usernames, chat) treated as untrusted.
- **CORS:** Strict allows only from trusted frontend domains.
