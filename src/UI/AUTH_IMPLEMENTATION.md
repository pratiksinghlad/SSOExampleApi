# Authentication & Token Storage

This document explains the authentication and token storage implementation following industry standards.

## 🔐 Security Architecture

### JWT Token Validation

- **Format Validation**: All tokens are validated to ensure they have exactly 3 parts separated by dots (JWS Compact Serialization)
- **Claims Validation**: Required claims (`exp`, `iat`, `sub`/`oid`) are checked before using tokens
- **Automatic Cleanup**: Invalid tokens are automatically removed from storage

### Token Storage Strategy (Industry Standards)

| Token Type        | Storage Location | Rationale                                                             |
| ----------------- | ---------------- | --------------------------------------------------------------------- |
| **Access Token**  | `sessionStorage` | Short-lived, cleared when tab closes                                  |
| **ID Token**      | `sessionStorage` | Contains user info, cleared when tab closes                           |
| **Refresh Token** | `localStorage`   | Persistent across sessions (httpOnly cookies preferred in production) |

## 🛠 Implementation Details

### Token Storage Service (`tokenStorageService.ts`)

```typescript
// Store tokens securely
tokenStorageService.storeTokens({
  accessToken: "jwt_token_here",
  idToken: "id_token_here",
  expiresAt: expiryTimestamp,
  scopes: ["User.Read", "openid"],
});

// Retrieve with automatic validation
const token = tokenStorageService.getAccessToken(); // null if invalid/expired
```

### Authentication Hook (`useAuth.ts`)

```typescript
const { isAuthenticated, user, login, logout } = useAuth();

// Automatically restores session on page load
// Validates stored tokens
// Provides reactive authentication state
```

### HTTP Interceptor (`httpInterceptor.ts`)

```typescript
// Automatically adds Bearer tokens to requests
// Validates JWT format before sending
// Handles token refresh on 401 errors
// Clears invalid tokens automatically
```

## 🚀 Usage Examples

### Basic Authentication Flow

```typescript
// Login
await login(); // Shows popup, stores tokens securely

// Check authentication
if (isAuthenticated) {
  // User is logged in with valid tokens
}

// Make authenticated API calls
const response = await apiClient.get("/me"); // Token added automatically
```

### Manual Token Management

```typescript
// Get current token info
const claims = tokenStorageService.getTokenClaims();
const userInfo = tokenStorageService.getUserInfo();

// Check expiry
const isExpired = tokenStorageService.isTokenExpired();

// Clear specific tokens
tokenStorageService.clearAccessToken();
tokenStorageService.clearAllTokens();
```

## 🔧 Configuration

### Environment Variables (`.env`)

```bash
VITE_CLIENT_ID=your-azure-app-client-id
VITE_REDIRECT_URI=http://localhost:5173
VITE_API_BASE_URL=https://localhost:5001/api
```

### MSAL Configuration (`authConfig.ts`)

```typescript
export const msalConfig = {
  auth: {
    clientId: import.meta.env.VITE_CLIENT_ID,
    authority: "https://login.microsoftonline.com/consumers",
    redirectUri: import.meta.env.VITE_REDIRECT_URI,
  },
  cache: {
    cacheLocation: "sessionStorage", // MSAL uses sessionStorage
    storeAuthStateInCookie: false,
  },
};
```

## 🛡 Security Features

### JWT Format Validation

- Validates 3-part structure (header.payload.signature)
- Base64URL decoding with error handling
- Automatic token cleanup on format errors

### Expiry Management

- 5-minute buffer before token expiry
- Automatic token refresh on API calls
- Session restoration on page reload

### Error Handling

- Graceful degradation for invalid tokens
- Automatic logout on repeated auth failures
- Clear error messages for debugging

## 📱 Session Persistence

### Page Reload Behavior

1. Check `sessionStorage` for valid access token
2. Check MSAL cache for account info
3. Attempt token refresh if account exists
4. Restore user session automatically

### Cross-Tab Behavior

- `sessionStorage`: Each tab has independent session
- `localStorage`: Shared refresh tokens across tabs
- MSAL cache: Shared account state

## 🔄 Token Refresh Flow

```
1. API call returns 401 Unauthorized
2. Check if token refresh is already in progress
3. Queue additional requests during refresh
4. Attempt silent token refresh
5. On success: retry all queued requests
6. On failure: logout user, clear tokens
```

## 🏗 Production Considerations

### Security Enhancements

- Use `httpOnly` cookies for refresh tokens
- Implement CSRF protection
- Add request signing for sensitive operations
- Consider token encryption at rest

### Performance Optimizations

- Token caching strategies
- Background token refresh
- Request deduplication during refresh

### Monitoring

- Token refresh failure tracking
- Authentication error logging
- Session duration analytics
