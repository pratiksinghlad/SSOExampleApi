# Azure AD Application Setup Guide for SSO Example API

This guide walks you through setting up an Azure Active Directory application for the SSO Example API with OAuth2 integration and Swagger UI authentication.

## Overview

This setup enables:
- OAuth2 authentication flow with Azure AD
- Swagger UI with integrated Azure AD authentication
- Personal Microsoft account support (Outlook.com, Live.com, etc.)
- Automatic token acquisition and API access
- Pre-configured Swagger client credentials

## Step 1: Access Azure Portal

1. Navigate to [Azure Portal](https://portal.azure.com)
2. Sign in with your Microsoft account
3. Search for "Azure Active Directory" in the search bar
4. Click on "Azure Active Directory"

## Step 2: Register a New Application

1. In the Azure AD overview page, click on **App registrations** in the left sidebar
2. Click **+ New registration**
3. Fill out the registration form:

### Basic Information
- **Name**: `SSOExampleApi`
- **Supported account types**: 
  - Select **"Personal Microsoft accounts only (personal Microsoft accounts - Skype, Xbox, Outlook.com)"**
  - This allows any Outlook.com, Live.com, or personal Microsoft account to sign in

### Redirect URI
- **Platform**: Web
- **Redirect URI**: `https://localhost:5001/signin-oidc`
  - Replace `5001` with your actual port if different
  - For production, use your actual domain

**Important for Swagger OAuth2**: Also add these redirect URIs:
- `https://localhost:5001/signout-callback-oidc`
- `https://localhost:5001/swagger/oauth2-redirect.html`

4. Click **Register**

## Step 3: Note Application Details

After registration, you'll see the application overview page. **Copy and save these values**:

- **Application (client) ID**: Found on the overview page
- **Directory (tenant) ID**: Found on the overview page
- **Object ID**: Also on the overview page (for reference)

## Step 4: Create Client Secret

1. In your application page, click on **Certificates & secrets** in the left sidebar
2. Click **+ New client secret**
3. Fill out the form:
   - **Description**: `SSOExampleApi Secret`
   - **Expires**: Choose appropriate expiration (6 months, 12 months, etc.)
4. Click **Add**
5. **IMPORTANT**: Copy the secret **Value** immediately - it will not be shown again!

## Step 5: Configure Authentication Settings

1. Click on **Authentication** in the left sidebar
2. Add the following redirect URIs (ALL are required):
   - `https://localhost:5001/signin-oidc`
   - `https://localhost:5001/signout-callback-oidc`
   - `https://localhost:5001/swagger/oauth2-redirect.html` (for Swagger OAuth2)

**Important**: The Swagger OAuth2 redirect URI (`/swagger/oauth2-redirect.html`) is essential for OAuth2 authentication to work in Swagger UI.

### Advanced Settings
3. Under **Implicit grant and hybrid flows**, check:
   - ✅ **Access tokens** (used for API access)
   - ✅ **ID tokens** (used for sign-in)

4. Under **Supported account types**, ensure:
   - ✅ **Personal Microsoft accounts only** is selected

5. Click **Save**

## Step 6: Configure API Permissions

1. Click on **API permissions** in the left sidebar
2. Remove any existing permissions and add these specific permissions:
3. Click **+ Add a permission**
4. Select **Microsoft Graph**
5. Choose **Delegated permissions**
6. Add these permissions:
   - `openid` (Sign users in)
   - `profile` (View users' basic profile)
   - `email` (View users' email address)
   - `User.Read` (Sign in and read user profile)

7. Click **Add permissions**

### For API Access (Custom Scope)
8. Click **+ Add a permission** again
9. Select **APIs my organization uses**
10. Find and select your registered application
11. Choose **Delegated permissions**
12. Add the custom scope: `access_as_user`

## Step 7: Expose an API (Custom Scope Configuration)

1. Click on **Expose an API** in the left sidebar
2. Click **+ Add a scope**
3. Accept the default Application ID URI or customize it
4. Fill out the scope details:
   - **Scope name**: `access_as_user`
   - **Admin consent display name**: `Access the API as the authenticated user`
   - **Admin consent description**: `Allow the application to access the API on behalf of the signed-in user`
   - **User consent display name**: `Access the API as you`
   - **User consent description**: `Allow the application to access the API on your behalf`
   - **State**: Enabled

5. Click **Add scope**

## Step 8: Configure Token Configuration

1. Click on **Token configuration** in the left sidebar
2. Click **+ Add optional claim**
3. Choose **ID** token type
4. Select these claims:
   - `email`
   - `family_name`
   - `given_name`
   - `preferred_username`
5. Click **Add**

6. Repeat for **Access** token type with the same claims
7. Click **Add**

## Step 9: Update Application Configuration

Take the values you copied and update your `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "consumers",
    "ClientId": "YOUR_CLIENT_ID_FROM_STEP_3", 
    "ClientSecret": "YOUR_CLIENT_SECRET_FROM_STEP_4",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc",
    "SignOutUrl": "https://login.microsoftonline.com/consumers/oauth2/v2.0/logout",
    "Scopes": [
      "openid",
      "profile",
      "email"
    ]
  }
}
```

**Important Configuration Notes:**
- **TenantId**: Set to `"consumers"` for personal Microsoft accounts (not your actual tenant ID)
- **SignOutUrl**: Uses `/consumers/` endpoint instead of `/common/`
- **Instance**: Keep as `"https://login.microsoftonline.com/"`

This configuration ensures your application uses the correct endpoints for personal Microsoft accounts (Outlook.com, Live.com, etc.) and avoids the AADSTS9002346 error.

## Step 10: Test the Swagger OAuth2 Integration

1. Start your application: `dotnet run`
2. Navigate to `https://localhost:5001` (Swagger UI opens automatically)
3. You'll see the OAuth2 authorization section in Swagger UI
4. Click on the **Authorize** button (lock icon) in Swagger UI
5. The OAuth2 authorization dialog will appear with:
   - **Client ID**: Pre-filled and disabled (cannot be modified)
   - **Scopes**: Shows available scopes (openid, profile, email, access_as_user)
6. Click **Authorize** - you'll be redirected to Microsoft's login page
7. Sign in with your personal Microsoft account (Outlook.com, Live.com, etc.)
8. Grant consent for the requested permissions
9. You'll be redirected back to Swagger UI with authentication complete
10. The **Authorize** button will now show as authenticated
11. Test any protected API endpoints - they will include the Bearer token automatically

## Swagger OAuth2 Features

The current configuration provides:

### Pre-configured Settings
- **Client ID**: Automatically filled from configuration and disabled for editing
- **PKCE**: Enabled for enhanced security
- **Scope Separator**: Space character (standard OAuth2)
- **Persistent Authorization**: Tokens are remembered across browser sessions

### Available Scopes
- `openid`: Basic authentication scope
- `profile`: Access to user's basic profile information
- `email`: Access to user's email address
- `api://YOUR_CLIENT_ID/access_as_user`: Custom scope for API access

### Security Features
- Authorization Code flow with PKCE
- Automatic token refresh handling
- Secure token storage in browser
- Pre-configured redirect URIs

## API Endpoint Authentication

After authenticating through Swagger, all API calls will automatically include:
```
Authorization: Bearer <access_token>
```

The token contains:
- User identity information
- Granted scopes
- Expiration time
- Application context

## Production Considerations

### Security
- Store the client secret securely (Azure Key Vault, environment variables)
- Use HTTPS in production
- Regularly rotate client secrets
- Monitor sign-in logs in Azure AD
- Implement proper token validation

### Redirect URIs
- Add your production domain to redirect URIs
- Remove localhost URIs in production
- Use consistent URI patterns
- Include Swagger OAuth2 redirect: `https://yourdomain.com/swagger/oauth2-redirect.html`

### Scopes and Permissions
- Review and minimize required permissions
- Implement proper consent flows
- Consider incremental consent for optional scopes
- Document all required permissions for end users

## Troubleshooting

### Common Issues

1. **"Invalid redirect URI"**
   - Ensure all redirect URIs are configured in Azure AD
   - Check for exact matches including trailing slashes, HTTP vs HTTPS, port numbers
   - Verify Swagger OAuth2 redirect URI is included

2. **"Application not found"**
   - Verify the Client ID is correct
   - Ensure the application is registered in the correct tenant

3. **"Invalid client secret"**
   - Check if the client secret has expired
   - Verify you copied the secret value, not the secret ID

4. **"Insufficient privileges"**
   - Check API permissions are granted
   - Verify user consent for the requested scopes
   - Ensure custom scope is properly exposed

5. **Swagger OAuth2 Issues**
   - Clear browser cache and cookies
   - Check if popup blockers are preventing OAuth2 window
   - Verify redirect URI includes `/swagger/oauth2-redirect.html`
   - Check browser console for JavaScript errors

6. **AADSTS9002346 Error - "Application is configured for use by Microsoft Account users only"**
   - **Cause**: Your app is configured for personal accounts but using organizational endpoints
   - **Solution**: Set `TenantId` to `"consumers"` in `appsettings.json`
   - **Update SignOut URL**: Use `/consumers/` instead of `/common/` or tenant-specific URLs
   - **Verify Account Type**: Ensure Azure AD app registration is set to "Personal Microsoft accounts only"
   - **Check Swagger Configuration**: Ensure OAuth2 URLs use `/consumers/` endpoints

7. **Invalid Redirect URI Error**
   - **Error**: "The provided value for the input parameter 'redirect_uri' is not valid"
   - **Solution**: Add the missing redirect URI to Azure AD app registration
   - **Required URIs**: Add `https://localhost:5001/swagger/oauth2-redirect.html` to your app registration
   - **Location**: Azure Portal → App registrations → Your app → Authentication → Redirect URIs
   - **Important**: ALL redirect URIs must be added exactly as shown (case-sensitive)

### Debug Tips
- Use browser developer tools to inspect authentication flows
- Check the Azure AD sign-in logs: Azure Portal > Azure AD > Sign-ins
- Enable detailed logging in your application
- Use [jwt.io](https://jwt.io) to decode and inspect JWT tokens
- Monitor network traffic during OAuth2 flow
- Check Swagger UI console for OAuth2 debug information

## Testing OAuth2 Flow

### Manual Testing Steps
1. Open Swagger UI in private/incognito browser window
2. Click Authorize button
3. Verify client ID is pre-filled and disabled
4. Select desired scopes (or leave all selected)
5. Click Authorize - should redirect to Microsoft login
6. Login with test Microsoft account
7. Grant consent for permissions
8. Verify successful redirect back to Swagger
9. Test protected API endpoints
10. Verify Bearer token is included in requests

### Automated Testing
```bash
# Test OAuth2 discovery endpoint
curl https://login.microsoftonline.com/YOUR_TENANT_ID/v2.0/.well-known/openid_configuration

# Test token endpoint (after obtaining authorization code)
curl -X POST https://login.microsoftonline.com/YOUR_TENANT_ID/oauth2/v2.0/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=YOUR_CLIENT_ID&scope=openid profile email&code=YOUR_AUTH_CODE&redirect_uri=YOUR_REDIRECT_URI&grant_type=authorization_code&code_verifier=YOUR_CODE_VERIFIER"
```

## Next Steps

After completing the Azure AD and Swagger setup:

### Development
1. Test all authentication flows
2. Implement proper error handling for authentication failures
3. Add comprehensive logging and monitoring
4. Consider implementing refresh token flows
5. Plan for token lifecycle management

### Security Enhancements
1. Implement token validation middleware
2. Add rate limiting for authentication endpoints
3. Monitor and log authentication events
4. Implement proper session management
5. Consider additional security headers

### User Experience
1. Customize OAuth2 consent experience
2. Implement user-friendly error messages
3. Add loading states during authentication
4. Consider single logout functionality
5. Implement user profile management

### Monitoring and Analytics
1. Set up Azure AD sign-in monitoring
2. Implement application-level authentication metrics
3. Monitor token usage and refresh patterns
4. Track user authentication journeys
5. Set up alerts for authentication failures
