# 🔐 SECURITY POLICY – RFID/NFC Object Tracking System

This policy outlines the current and planned security model for the project's backend API.

## Phase 1 – Minimal Viable Security (Development)
- ❗ No authentication required (temporary for internal testing only)
- Only trusted users (Tony, Francesco, Enrico) are aware of the deployment URL
- API access is limited to trusted devices via obscurity (not secure)

## Phase 2 – Staging/Pre-production (Optional Security)
- Basic API key or token authentication
- IP-based filtering if supported by host
- Simple HTTPS deployment via secure platform

## Phase 3 – Production-Ready
- ✅ OAuth 2.0 with identity provider (e.g., Auth0, Firebase Auth, or Keycloak)
- HTTPS enforced
- Rate limiting and logging
- Authentication for both read and write actions
- Role-based access control (RBAC) for user types (owner/admin/viewer)

## Notes
For now, do **not expose the API to the public** without at least a token-based access model.
