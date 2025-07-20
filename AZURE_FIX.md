# Azure AD Configuration Fix for Swagger Authentication

## Required Azure AD Settings

### 1. Application Platform Configuration
In Azure Portal → App Registrations → Your App → Authentication:

**Platform Configuration:**
- **Platform Type**: Single-page application (SPA)
- **Redirect URIs**: 
  - `https://localhost:5001/swagger/oauth2-redirect.html`
  - `https://localhost:5001/signin-oidc` (if needed for other flows)
  - `https://localhost:5001/signout-callback-oidc` (if needed for other flows)

### 2. Supported Account Types
- Set to: **Personal Microsoft accounts only**

### 3. Authentication Flow Settings
**Advanced settings:**
- ✅ **Allow public client flows**: YES
- ✅ **Enable the following mobile and desktop flows**: YES

**Implicit grant and hybrid flows:**
- ✅ **Access tokens** (used for implicit flows)
- ✅ **ID tokens** (used for implicit and hybrid flows)

### 4. API Permissions
Make sure these permissions are granted:
- Microsoft Graph → User.Read (Delegated)
- Microsoft Graph → openid (Delegated)
- Microsoft Graph → profile (Delegated)
- Microsoft Graph → email (Delegated)

## Key Changes Made to Fix Issues

### 1. Fixed Swagger Pipeline Order
- Moved Swagger configuration before authentication middleware
- This fixes the 401 error when loading `/swagger/v1/swagger.json`

### 2. Changed to Authorization Code Flow with PKCE
- Switched from Implicit flow to Authorization Code flow with PKCE
- This is the recommended and more secure approach for SPAs
- Added `options.OAuthUsePkce()` to enable PKCE

### 3. Simplified Scope Configuration
- Removed Microsoft Graph scopes from Swagger config
- Using only OpenID Connect scopes for authentication
- This avoids scope mismatch issues

## Testing Steps

1. **Update Azure AD Configuration**: Follow the platform settings above
2. **Clear Browser Cache**: Clear all browser data for localhost
3. **Restart Application**: Stop and restart your .NET application
4. **Test Swagger UI**: Go to `https://localhost:5001/swagger`
5. **Click Authorize**: Should redirect to Microsoft login
6. **Sign in**: Use your personal Microsoft account
7. **Grant Permissions**: Allow the requested permissions
8. **Verify Token**: Should be redirected back with valid token

## Troubleshooting

### If you still get redirect_uri errors:
1. Double-check the redirect URI in Azure AD exactly matches: `https://localhost:5001/swagger/oauth2-redirect.html`
2. Make sure the platform is set to "Single-page application (SPA)"
3. Clear browser cache completely
4. Try in incognito/private browsing mode

### If authentication fails:
1. Check that "Allow public client flows" is enabled
2. Verify the account type is set to "Personal Microsoft accounts only"
3. Ensure all required permissions are granted and admin consent is provided if needed

### If you get 401 errors:
1. Make sure Swagger is configured before authentication middleware (already fixed)
2. Check that the application is running on HTTPS
3. Verify the client ID in appsettings.json matches Azure AD
