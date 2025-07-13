# SSO Example API

A comprehensive .NET 9 Web API demonstrating Azure Active Directory Single Sign-On (SSO) integration using OAuth 2.0 and OpenID Connect protocols with integrated Swagger UI authentication.

## Features

- **Azure AD OAuth2 Integration**: Full OAuth 2.0 and OpenID Connect implementation
- **Interactive Swagger Authentication**: Built-in OAuth2 authentication directly in Swagger UI
- **Personal Account Support**: Works with any Outlook.com, Live.com, and personal Microsoft accounts
- **Pre-configured Swagger UI**: Client ID auto-filled and disabled for security
- **JWT Token Validation**: Comprehensive token handling and validation
- **User Profile Management**: Extract and manage user information from Azure AD claims
- **Permission-based Authorization**: Role and permission checking capabilities
- **Real-time API Testing**: Test authenticated endpoints directly in Swagger UI
- **Modern C# 13**: Uses latest .NET 9 and C# 13 features and best practices

## Quick Start

1. **Clone and configure** the project
2. **Set up Azure AD** following the [Azure Setup Guide](AZURE_SETUP.md)
3. **Update configuration** in `appsettings.json`
4. **Run the application**: `dotnet run`
5. **Open Swagger UI**: Navigate to `https://localhost:5001`
6. **Click Authorize** in Swagger UI and login with your Microsoft account
7. **Test API endpoints** directly in the authenticated Swagger interface

## Swagger OAuth2 Integration

### Key Features
- **One-Click Authentication**: Authorize button redirects to Microsoft login
- **Auto-filled Client ID**: Pre-configured and disabled for security
- **Persistent Sessions**: Authentication state maintained across browser sessions
- **Real-time Testing**: All protected endpoints automatically include Bearer tokens
- **Scope Management**: Visual scope selection during authorization
- **Token Information**: View token details and expiration

### Authentication Flow
1. Click **Authorize** button in Swagger UI
2. Redirect to Microsoft OAuth2 login page
3. Enter Outlook.com/Live.com credentials
4. Grant consent for requested permissions
5. Redirect back to Swagger UI with active authentication
6. Test any API endpoint with automatic token inclusion

## Project Structure

```
SSOExampleApi/
├── Configuration/
│   └── AzureAdOptions.cs          # Azure AD configuration model
├── Controllers/
│   ├── AuthController.cs          # Authentication endpoints
│   ├── HomeController.cs          # Basic API info endpoints
│   └── UserController.cs          # User management endpoints
├── Models/
│   ├── AuthenticationResult.cs    # Authentication operation results
│   ├── TokenInfo.cs              # Token information model
│   └── UserInfo.cs               # User profile model
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── ITokenService.cs
│   │   └── IUserService.cs
│   ├── AuthService.cs            # Authentication logic
│   ├── TokenService.cs           # JWT token handling
│   └── UserService.cs            # User profile management
├── Program.cs                    # Application entry point
├── Startup.cs                    # Main application configuration
├── Startup.Swagger.cs            # Swagger OAuth2 configuration
└── appsettings.json             # Configuration settings
```

## Prerequisites

- .NET 9 SDK
- Azure Active Directory (Azure AD) tenant or personal Microsoft account
- Basic understanding of OAuth2 and JWT tokens

## Azure AD Setup

### Quick Setup Summary

1. **Register Application** in [Azure Portal](https://portal.azure.com)
2. **Configure Redirect URIs** including Swagger OAuth2 callback
3. **Create Client Secret** and note the value
4. **Set API Permissions** for Microsoft Graph
5. **Expose Custom API Scope** for your application
6. **Update Configuration** in your application

For detailed step-by-step instructions, see the [Complete Azure Setup Guide](AZURE_SETUP.md).

### Required Redirect URIs
```
https://localhost:5001/signin-oidc
https://localhost:5001/signout-callback-oidc
https://localhost:5001/swagger/oauth2-redirect.html
```

**Important**: ALL redirect URIs above must be added to your Azure AD app registration exactly as shown.

### Required Scopes
- `openid` - Basic authentication
- `profile` - User profile information  
- `email` - User email address
- `api://YOUR_CLIENT_ID/access_as_user` - Custom API access

## Configuration

Update the `appsettings.json` file with your Azure AD application details:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "consumers",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
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

Replace the following placeholders:
- `YOUR_CLIENT_ID`: Your registered application's client ID  
- `YOUR_CLIENT_SECRET`: Your application's client secret

**Important**: For personal Microsoft accounts, use `"consumers"` as the TenantId, not your actual tenant ID.

## Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Build the application**:
   ```bash
   dotnet build
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access the application**:
   - **Swagger UI**: `https://localhost:5001` (opens automatically)
   - **API endpoints**: `https://localhost:5001/api/`

## Using the Swagger OAuth2 Interface

### Step 1: Access Swagger UI
Navigate to `https://localhost:5001` - Swagger UI opens as the default page.

### Step 2: Authenticate
1. Click the **Authorize** button (lock icon) in the top-right of Swagger UI
2. In the OAuth2 dialog:
   - **Client ID**: Pre-filled and disabled (cannot be modified)
   - **Scopes**: Select desired scopes (recommended: select all)
3. Click **Authorize**
4. You'll be redirected to Microsoft's login page
5. Sign in with your Outlook.com, Live.com, or personal Microsoft account
6. Grant consent for the requested permissions
7. You'll be redirected back to Swagger UI

### Step 3: Test API Endpoints  
- All protected endpoints now automatically include your Bearer token
- Click **Try it out** on any endpoint to test with your authentication
- View request/response details including the Authorization header

### Step 4: Logout (Optional)
- Click **Authorize** again and then **Logout** to clear authentication
- Or close the browser to end the session

## API Endpoints

### Authentication Endpoints

- `GET /api/auth/login` - Initiate login with Azure AD
- `GET /api/auth/login-callback` - Handle Azure AD callback  
- `POST /api/auth/logout` - Sign out from Azure AD
- `GET /api/auth/status` - Get authentication status
- `POST /api/auth/validate-token` - Validate JWT token

### User Endpoints (🔒 Requires Authentication)

- `GET /api/user/profile` - Get current user profile
- `GET /api/user/{userId}` - Get user by ID
- `GET /api/user/token-info` - Get token information
- `GET /api/user/claims` - Get user claims
- `GET /api/user/roles` - Get user roles and groups
- `POST /api/user/check-permissions` - Check user permissions

### General Endpoints

- `GET /` - API information
- `GET /health` - Health check
- `GET /error` - Error handling

## Testing Authentication in Swagger

### Manual Testing Process
1. **Start application**: `dotnet run`
2. **Open Swagger**: Navigate to `https://localhost:5001`
3. **Authenticate**: Click Authorize → Login with Microsoft account
4. **Test endpoints**: Try protected endpoints like `/api/user/profile`
5. **View tokens**: Check `/api/user/token-info` to see JWT details
6. **Inspect requests**: See automatic Authorization header inclusion

### Example: Testing User Profile
1. After authentication, expand `GET /api/user/profile`
2. Click **Try it out**
3. Click **Execute** 
4. View the response containing your Microsoft account information
5. Check the **Request details** to see the Bearer token header

## Security Features

- **OAuth2 Authorization Code Flow**: Secure authentication flow with PKCE
- **JWT Token Validation**: Comprehensive token parsing and validation
- **Claims-based Authorization**: Extract user information from Azure AD claims
- **Automatic Token Inclusion**: Swagger UI automatically adds Bearer tokens
- **Persistent Authentication**: Session maintained across page refreshes
- **Secure Token Storage**: Tokens stored securely in browser session
- **HTTPS Enforcement**: All communications over secure channels

## Development Features

- **Real-time API Testing**: Test authenticated endpoints immediately after login
- **Interactive Documentation**: Swagger UI with live authentication
- **Pre-configured Security**: Client credentials automatically managed
- **Developer Friendly**: Clear error messages and debugging support
- **Modern Architecture**: .NET 9 with C# 13 features

## Troubleshooting

### Authentication Issues

1. **"Invalid redirect URI"**
   - Verify all redirect URIs are configured in Azure AD
   - Check exact URL match including `/swagger/oauth2-redirect.html`
   - Ensure HTTPS and correct port numbers

2. **"Application not found"**
   - Verify Client ID in `appsettings.json`
   - Check application is registered in correct tenant

3. **Swagger OAuth2 not working**
   - Clear browser cache and cookies
   - Check popup blockers aren't preventing OAuth2 window
   - Verify JavaScript is enabled

4. **Token validation errors**
   - Check client secret hasn't expired
   - Verify tenant ID is correct
   - Ensure API permissions are granted

5. **AADSTS9002346 Error (Personal Microsoft Accounts)**
   - **Error**: "Application is configured for use by Microsoft Account users only"
   - **Solution**: Set `TenantId` to `"consumers"` in `appsettings.json`
   - **Check**: Ensure `SignOutUrl` uses `/consumers/` endpoint
   - **Verify**: Azure AD app registration supports "Personal Microsoft accounts only"

6. **Invalid Redirect URI Error**
   - **Error**: "The provided value for the input parameter 'redirect_uri' is not valid"
   - **Solution**: Add missing redirect URI to Azure AD app registration
   - **Required**: Add `https://localhost:5001/swagger/oauth2-redirect.html`
   - **Location**: Azure Portal → Your App → Authentication → Redirect URIs

### Debug Tips

- **Azure AD Logs**: Check sign-in logs in Azure Portal
- **Browser Console**: Look for JavaScript errors during OAuth2 flow
- **Network Tab**: Inspect OAuth2 requests and responses
- **JWT Decoder**: Use [jwt.io](https://jwt.io) to examine token contents
- **Application Logs**: Enable detailed logging in `appsettings.Development.json`

### Common Solutions

- **Clear browser data** if authentication seems cached incorrectly
- **Try incognito/private browsing** to test fresh authentication
- **Verify scopes** match between Azure AD and application configuration
- **Check certificate trust** for localhost HTTPS development

## Production Deployment

### Security Checklist
- [ ] Store client secret in Azure Key Vault or secure environment variables
- [ ] Update redirect URIs to production domains
- [ ] Remove localhost URIs from Azure AD configuration
- [ ] Enable proper HTTPS certificates
- [ ] Implement token refresh strategies
- [ ] Add comprehensive logging and monitoring
- [ ] Configure proper CORS policies
- [ ] Implement rate limiting

### Configuration Updates
- Update `appsettings.Production.json` with production values
- Configure proper redirect URIs for your domain
- Set up Azure Key Vault for secret management
- Configure production logging providers

## Additional Resources

- [Azure AD Documentation](https://docs.microsoft.com/en-us/azure/active-directory/)
- [OAuth 2.0 Specification](https://tools.ietf.org/html/rfc6749)
- [JWT.io Token Decoder](https://jwt.io)
- [Complete Azure Setup Guide](AZURE_SETUP.md)
- [Swagger UI Documentation](https://swagger.io/tools/swagger-ui/)

## License

This project is provided as an example for educational and development purposes.

## OAuth Flow Diagram

```mermaid
flowchart LR
    A[Swagger UI: User clicks "Authorize"] --> B[Browser: Redirect to Azure AD authorize endpoint]
    B --> C[Azure AD: User signs in & grants consent]
    C --> D[Azure AD: Redirect back to Swagger UI callback with access token]
    D --> E[Swagger UI: Stores token and is authorized]
    E --> F[Swagger UI: Calls GET /api/user/profile with Bearer token]
    F --> G[API Backend: Validates JWT token]
    G --> H[API Backend: Returns 200 OK with user profile]
```
