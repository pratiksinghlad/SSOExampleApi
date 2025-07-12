# Azure AD Application Setup Guide

This guide walks you through setting up an Azure Active Directory application for the SSO Example API.

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
2. Add additional redirect URIs if needed:
   - `https://localhost:5001/signin-oidc`
   - `https://localhost:5001/signout-callback-oidc`
   - For production: `https://yourdomain.com/signin-oidc`

### Advanced Settings
3. Under **Implicit grant and hybrid flows**, check:
   - ✅ **Access tokens** (used for API access)
   - ✅ **ID tokens** (used for sign-in)

4. Under **Supported account types**, ensure:
   - ✅ **Personal Microsoft accounts only** is selected

5. Click **Save**

## Step 6: Configure API Permissions

1. Click on **API permissions** in the left sidebar
2. You should see **Microsoft Graph** with **User.Read** permission already added
3. Click **+ Add a permission**
4. Select **Microsoft Graph**
5. Choose **Delegated permissions**
6. Add these permissions:
   - `openid` (should already be present)
   - `profile` (should already be present)  
   - `email` (add this one)
   - `User.Read` (should already be present)

7. Click **Add permissions**
8. Optionally, click **Grant admin consent** if you have admin rights

## Step 7: Configure Token Configuration (Optional)

1. Click on **Token configuration** in the left sidebar
2. Click **+ Add optional claim**
3. Choose **ID**
4. Select useful claims like:
   - `email`
   - `family_name`
   - `given_name`
   - `preferred_username`
5. Click **Add**

## Step 8: Update Application Configuration

Take the values you copied and update your `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "YOUR_DOMAIN.onmicrosoft.com",
    "TenantId": "YOUR_TENANT_ID_FROM_STEP_3",
    "ClientId": "YOUR_CLIENT_ID_FROM_STEP_3", 
    "ClientSecret": "YOUR_CLIENT_SECRET_FROM_STEP_4",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc",
    "SignOutUrl": "https://login.microsoftonline.com/common/oauth2/v2.0/logout"
  }
}
```

## Step 9: Test the Configuration

1. Start your application: `dotnet run`
2. Navigate to `https://localhost:5001`
3. Try the login endpoint: `GET /api/auth/login`
4. You should be redirected to Microsoft's login page
5. Sign in with any personal Microsoft account (Outlook.com, Live.com, etc.)
6. After successful authentication, you should be redirected back to your application

## Production Considerations

### Security
- Store the client secret securely (Azure Key Vault, environment variables)
- Use HTTPS in production
- Regularly rotate client secrets
- Monitor sign-in logs in Azure AD

### Redirect URIs
- Add your production domain to redirect URIs
- Remove localhost URIs in production
- Use consistent URI patterns

### Permissions
- Review and minimize required permissions
- Consider using application permissions for background services
- Implement proper consent flows

## Troubleshooting

### Common Issues

1. **"Invalid redirect URI"**
   - Ensure the redirect URI exactly matches what's configured in Azure AD
   - Check for trailing slashes, HTTP vs HTTPS, port numbers

2. **"Application not found"**
   - Verify the Client ID is correct
   - Ensure the application is registered in the correct tenant

3. **"Invalid client secret"**
   - Check if the client secret has expired
   - Verify you copied the secret value, not the secret ID

4. **"Insufficient privileges"**
   - Check API permissions are granted
   - Verify user consent for the requested scopes

### Debug Tips
- Use browser developer tools to inspect authentication flows
- Check the Azure AD sign-in logs: Azure Portal > Azure AD > Sign-ins
- Enable detailed logging in your application
- Use [jwt.io](https://jwt.io) to decode and inspect JWT tokens

## Next Steps

After completing the Azure AD setup:
1. Test all authentication flows
2. Implement proper error handling
3. Add logging and monitoring
4. Consider implementing refresh token flows
5. Plan for token lifecycle management
