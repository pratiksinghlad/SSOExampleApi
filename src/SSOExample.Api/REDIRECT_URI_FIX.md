# URGENT: Fix for Redirect URI Error

## Problem
Getting error: "The provided value for the input parameter 'redirect_uri' is not valid"

## Root Cause
Missing Swagger OAuth2 redirect URI in Azure AD app registration.

## IMMEDIATE SOLUTION

### Step 1: Add Missing Redirect URI to Azure AD

1. **Go to Azure Portal**: https://portal.azure.com
2. **Navigate to**: Azure Active Directory → App registrations → Your app (SSOExampleApi)
3. **Click**: Authentication (left sidebar)
4. **Add Redirect URI**: Click "+ Add a URI"
5. **Enter EXACTLY**: `https://localhost:5001/swagger/oauth2-redirect.html`
6. **Click**: Save

### Step 2: Verify All Required Redirect URIs

Make sure you have ALL of these redirect URIs in your Azure AD app:

```
https://localhost:5001/signin-oidc
https://localhost:5001/signout-callback-oidc
https://localhost:5001/swagger/oauth2-redirect.html
```

### Step 3: Test the Fix

1. **Run your application**: `dotnet run`
2. **Open Swagger**: https://localhost:5001
3. **Click Authorize**: Should now work without redirect URI error
4. **Login**: Use your Outlook.com/Live.com account
5. **Test APIs**: Try any protected endpoint

## What Changed in Code

### Fixed OAuth2 Flow Type
- Changed from `Implicit` to `AuthorizationCode` flow for better security
- Removed custom API scope that wasn't needed for basic authentication
- Simplified to only use standard Microsoft Graph scopes

### Updated Configuration
- Ensured `TenantId` is set to `"consumers"` for personal Microsoft accounts
- Updated OAuth2 endpoints to use `/consumers/` instead of tenant-specific URLs

## Key Files Modified

1. **Startup.Swagger.cs**
   - Changed OAuth2 flow from Implicit to AuthorizationCode
   - Simplified scopes to just: openid, profile, email
   - Removed custom API scope

2. **appsettings.json**
   - Set `TenantId` to `"consumers"`
   - Updated `SignOutUrl` to use `/consumers/` endpoint

3. **Documentation**
   - Updated AZURE_SETUP.md with specific redirect URI requirements
   - Added troubleshooting sections for redirect URI errors

## Important Notes

- **Case Sensitive**: Redirect URIs must match exactly (case-sensitive)
- **Protocol**: Must use `https://` (not `http://`)
- **Port**: Must match your application's port (5001 in this case)
- **Path**: Must be exactly `/swagger/oauth2-redirect.html`

## Verification Checklist

- [ ] All 3 redirect URIs added to Azure AD app registration
- [ ] TenantId set to "consumers" in appsettings.json
- [ ] Application builds successfully
- [ ] Swagger UI loads without errors
- [ ] OAuth2 authentication works in Swagger
- [ ] Can login with personal Microsoft account
- [ ] Protected API endpoints work after authentication

## If Still Having Issues

1. **Clear browser cache** and try in incognito mode
2. **Double-check redirect URIs** in Azure AD (exact match required)
3. **Verify account type** in Azure AD app registration (should be "Personal Microsoft accounts only")
4. **Check browser console** for any JavaScript errors
5. **Review Azure AD sign-in logs** for additional error details

The redirect URI error should be completely resolved after adding the missing Swagger OAuth2 redirect URI to your Azure AD app registration.
