# API Testing Guide

This guide provides step-by-step instructions for testing the SSO Example API functionality.

## Prerequisites

1. **Azure AD Application configured** (see [AZURE_SETUP.md](AZURE_SETUP.md))
2. **Application running locally**:
   ```bash
   cd SSOExampleApi
   dotnet run
   ```
3. **Browser or HTTP client** (Postman, curl, etc.)

## Base URL

- Development: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## Testing Authentication Flow

### 1. Check API Information
```http
GET https://localhost:5001/
```

**Expected Response:**
```json
{
  "Name": "SSO Example API",
  "Version": "1.0.0",
  "Description": "A sample API demonstrating Azure AD Single Sign-On integration",
  "Endpoints": {
    "Authentication": {
      "Login": "/api/auth/login",
      "Logout": "/api/auth/logout",
      "Status": "/api/auth/status",
      "ValidateToken": "/api/auth/validate-token"
    },
    ...
  }
}
```

### 2. Health Check
```http
GET https://localhost:5001/health
```

**Expected Response:**
```json
{
  "Status": "Healthy",
  "Timestamp": "2025-01-12T...",
  "Version": "1.0.0",
  "Environment": "Development"
}
```

### 3. Check Authentication Status (Unauthenticated)
```http
GET https://localhost:5001/api/auth/status
```

**Expected Response:**
```json
{
  "IsAuthenticated": false,
  "UserName": null,
  "AuthenticationType": null,
  "Claims": []
}
```

### 4. Initiate Login
```http
GET https://localhost:5001/api/auth/login
```

**Expected Behavior:**
- Browser redirects to Microsoft login page
- User enters personal Microsoft account credentials
- After successful authentication, redirects back to the application

### 5. Check Authentication Status (Authenticated)
```http
GET https://localhost:5001/api/auth/status
```

**Expected Response (after login):**
```json
{
  "IsAuthenticated": true,
  "UserName": "user@outlook.com",
  "AuthenticationType": "Cookies",
  "Claims": [
    {
      "Type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "Value": "..."
    },
    ...
  ]
}
```

## Testing User Endpoints (Requires Authentication)

### 1. Get Current User Profile
```http
GET https://localhost:5001/api/user/profile
Authorization: Bearer [your-jwt-token]
```

**Expected Response:**
```json
{
  "Id": "00000000-0000-0000-0000-000000000000",
  "Email": "user@outlook.com",
  "DisplayName": "John Doe",
  "GivenName": "John",
  "Surname": "Doe",
  "JobTitle": "",
  "Roles": [],
  "Groups": [],
  "TenantId": "...",
  "AdditionalClaims": {}
}
```

### 2. Get User Claims
```http
GET https://localhost:5001/api/user/claims
Authorization: Bearer [your-jwt-token]
```

**Expected Response:**
```json
[
  {
    "Type": "aud",
    "Value": "your-client-id",
    "ValueType": "http://www.w3.org/2001/XMLSchema#string",
    "Issuer": "https://login.microsoftonline.com/..."
  },
  ...
]
```

### 3. Get User Roles
```http
GET https://localhost:5001/api/user/roles
Authorization: Bearer [your-jwt-token]
```

**Expected Response:**
```json
{
  "UserId": "00000000-0000-0000-0000-000000000000",
  "UserName": "John Doe",
  "Roles": [],
  "Groups": []
}
```

### 4. Check User Permissions
```http
POST https://localhost:5001/api/user/check-permissions
Content-Type: application/json
Authorization: Bearer [your-jwt-token]

["admin", "user", "read"]
```

**Expected Response:**
```json
{
  "HasPermissions": false,
  "RequestedPermissions": ["admin", "user", "read"],
  "UserId": "user@outlook.com"
}
```

### 5. Get Token Information
```http
GET https://localhost:5001/api/user/token-info
Authorization: Bearer [your-jwt-token]
```

**Expected Response:**
```json
{
  "AccessToken": "",
  "RefreshToken": "",
  "IdToken": "",
  "TokenType": "Bearer",
  "ExpiresAt": "2025-01-12T...",
  "Scope": "openid profile email",
  "AdditionalProperties": {
    "issuer": "https://login.microsoftonline.com/...",
    "audience": "your-client-id"
  },
  "IsExpired": false,
  "TimeUntilExpiration": "01:00:00"
}
```

## Testing Token Validation

### 1. Validate JWT Token
```http
POST https://localhost:5001/api/auth/validate-token
Content-Type: application/json

"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIs..."
```

**Expected Response (Valid Token):**
```json
{
  "IsValid": true,
  "IsExpired": false,
  "TokenInfo": {
    "ExpiresAt": "2025-01-12T...",
    "Scope": "openid profile email",
    ...
  },
  "Claims": [...]
}
```

**Expected Response (Invalid Token):**
```json
{
  "error": "Unauthorized",
  "message": "Invalid token"
}
```

## Testing Logout

### 1. Sign Out
```http
POST https://localhost:5001/api/auth/logout
Authorization: Bearer [your-jwt-token]
```

**Expected Behavior:**
- User is signed out from Azure AD
- Session cookies are cleared
- Redirect to sign-out callback

## Error Scenarios

### 1. Access Protected Endpoint Without Authentication
```http
GET https://localhost:5001/api/user/profile
```

**Expected Response:**
```http
HTTP/1.1 401 Unauthorized
```

### 2. Invalid Token
```http
POST https://localhost:5001/api/auth/validate-token
Content-Type: application/json

"invalid-jwt-token"
```

**Expected Response:**
```http
HTTP/1.1 401 Unauthorized
{
  "error": "Token validation failed"
}
```

### 3. Missing User ID
```http
GET https://localhost:5001/api/user/
```

**Expected Response:**
```http
HTTP/1.1 400 Bad Request
{
  "error": "User ID is required"
}
```

## Browser-Based Testing

### Complete Authentication Flow

1. **Open browser** and navigate to `https://localhost:5001`
2. **Click login link** or navigate to `/api/auth/login`
3. **Sign in** with your personal Microsoft account (Outlook.com, Live.com, etc.)
4. **Verify redirect** back to the application
5. **Test protected endpoints** using the authenticated session
6. **Sign out** using `/api/auth/logout`

### Swagger UI Testing

1. **Navigate to** `https://localhost:5001/swagger`
2. **Click "Authorize"** button
3. **Enter Bearer token** (obtain from browser developer tools after login)
4. **Test endpoints** directly in Swagger UI

## Postman Collection

You can import this collection for comprehensive testing:

```json
{
  "info": {
    "name": "SSO Example API",
    "description": "Test collection for Azure AD SSO API"
  },
  "item": [
    {
      "name": "Health Check",
      "request": {
        "method": "GET",
        "header": [],
        "url": {
          "raw": "{{baseUrl}}/health",
          "host": ["{{baseUrl}}"],
          "path": ["health"]
        }
      }
    },
    {
      "name": "Login",
      "request": {
        "method": "GET",
        "header": [],
        "url": {
          "raw": "{{baseUrl}}/api/auth/login",
          "host": ["{{baseUrl}}"],
          "path": ["api", "auth", "login"]
        }
      }
    },
    {
      "name": "User Profile",
      "request": {
        "method": "GET",
        "header": [
          {
            "key": "Authorization",
            "value": "Bearer {{token}}"
          }
        ],
        "url": {
          "raw": "{{baseUrl}}/api/user/profile",
          "host": ["{{baseUrl}}"],
          "path": ["api", "user", "profile"]
        }
      }
    }
  ],
  "variable": [
    {
      "key": "baseUrl",
      "value": "https://localhost:5001"
    },
    {
      "key": "token",
      "value": ""
    }
  ]
}
```

## Troubleshooting

### Common Issues

1. **Certificate Errors**: Trust the development certificate
   ```bash
   dotnet dev-certs https --trust
   ```

2. **Authentication Redirect Loops**: Check Azure AD redirect URIs configuration

3. **401 Unauthorized**: Ensure proper token format and authentication

4. **CORS Issues**: Add appropriate CORS policies if testing from different origin

### Debugging Tips

- **Enable detailed logging** in `appsettings.Development.json`
- **Use browser developer tools** to inspect authentication flows
- **Check Azure AD sign-in logs** for authentication issues
- **Validate JWT tokens** at [jwt.io](https://jwt.io)
- **Monitor application logs** for detailed error information

## Performance Testing

### Load Testing Endpoints

Use tools like Apache Bench or Artillery to test:

```bash
# Health check endpoint
ab -n 1000 -c 10 https://localhost:5001/health

# API info endpoint
ab -n 500 -c 5 https://localhost:5001/
```

### Monitoring

- **Response times** for all endpoints
- **Authentication flow performance**
- **Token validation speed**
- **Memory usage** under load
