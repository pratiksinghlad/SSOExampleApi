# SSO Example API

A comprehensive .NET 9 Web API demonstrating Azure Active Directory Single Sign-On (SSO) integration using OAuth 2.0 and OpenID Connect protocols.

## Features

- **Azure AD Integration**: Full OAuth 2.0 and OpenID Connect implementation
- **Personal Account Support**: Works with any Outlook.com, Live.com, and personal Microsoft accounts
- **JWT Token Validation**: Comprehensive token handling and validation
- **User Profile Management**: Extract and manage user information from Azure AD claims
- **Permission-based Authorization**: Role and permission checking capabilities
- **Swagger Documentation**: Interactive API documentation with authentication support
- **Modern C# 13**: Uses latest .NET 9 and C# 13 features and best practices

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
│   │   ├── IAuthenticationService.cs
│   │   ├── ITokenService.cs
│   │   └── IUserService.cs
│   ├── AuthenticationService.cs   # Authentication logic
│   ├── TokenService.cs           # JWT token handling
│   └── UserService.cs            # User profile management
├── Program.cs                    # Application entry point
├── Startup.cs                    # Application configuration
└── appsettings.json             # Configuration settings
```

## Prerequisites

- .NET 9 SDK
- Azure Active Directory (Azure AD) tenant
- Registered Azure AD application

## Azure AD Setup

### 1. Register Application in Azure Portal

1. Go to [Azure Portal](https://portal.azure.com)
2. Navigate to **Azure Active Directory** > **App registrations**
3. Click **New registration**
4. Fill in the registration form:
   - **Name**: `SSOExampleApi`
   - **Supported account types**: Select "Personal Microsoft accounts only"
   - **Redirect URI**: 
     - Type: Web
     - URI: `https://localhost:5001/signin-oidc` (adjust port if needed)

### 2. Configure Application Settings

After registration, note down:
- **Application (client) ID**
- **Directory (tenant) ID**

### 3. Create Client Secret

1. Go to **Certificates & secrets**
2. Click **New client secret**
3. Add description and set expiration
4. **Copy the secret value immediately** (it won't be shown again)

### 4. Configure Redirect URIs

1. Go to **Authentication**
2. Add redirect URIs:
   - `https://localhost:5001/signin-oidc`
   - `https://localhost:5001/signout-callback-oidc`
3. Under **Implicit grant and hybrid flows**, check:
   - Access tokens
   - ID tokens

### 5. Configure API Permissions

1. Go to **API permissions**
2. Ensure these Microsoft Graph permissions are granted:
   - `openid`
   - `profile`
   - `email`
   - `User.Read`

## Configuration

Update the `appsettings.json` file with your Azure AD application details:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "YOUR_DOMAIN.onmicrosoft.com",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc",
    "SignOutUrl": "https://login.microsoftonline.com/common/oauth2/v2.0/logout"
  }
}
```

Replace the following placeholders:
- `YOUR_DOMAIN`: Your Azure AD domain (e.g., `contoso.onmicrosoft.com`)
- `YOUR_TENANT_ID`: Your Azure AD tenant ID
- `YOUR_CLIENT_ID`: Your registered application's client ID
- `YOUR_CLIENT_SECRET`: Your application's client secret

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

4. **Access the API**:
   - Swagger UI: `https://localhost:5001`
   - API endpoints: `https://localhost:5001/api/`

## API Endpoints

### Authentication Endpoints

- `GET /api/auth/login` - Initiate login with Azure AD
- `GET /api/auth/login-callback` - Handle Azure AD callback
- `POST /api/auth/logout` - Sign out from Azure AD
- `GET /api/auth/status` - Get authentication status
- `POST /api/auth/validate-token` - Validate JWT token

### User Endpoints (Requires Authentication)

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

## Testing the Authentication Flow

1. **Start the application** and navigate to `https://localhost:5001`

2. **Initiate login** by calling `GET /api/auth/login`

3. **Complete Azure AD authentication** in the browser

4. **Access protected endpoints** using the authenticated session

5. **View user information** at `/api/user/profile`

## Security Features

- **JWT Token Validation**: Comprehensive token parsing and validation
- **Claims-based Authorization**: Extract user information from Azure AD claims
- **Role-based Access Control**: Support for checking user roles and permissions
- **Secure Token Handling**: Proper token lifecycle management
- **HTTPS Enforcement**: Secure communication protocols

## Development Notes

- **No Top-level Statements**: Uses traditional Program.cs and Startup.cs structure
- **Dependency Injection**: Comprehensive DI setup for all services
- **Logging**: Structured logging throughout the application
- **Error Handling**: Proper exception handling and error responses
- **API Documentation**: Swagger/OpenAPI integration with authentication support

## Troubleshooting

### Common Issues

1. **Authentication fails**: Verify Azure AD configuration and redirect URIs
2. **Token validation errors**: Check client secret and tenant ID
3. **Permission denied**: Ensure proper API permissions in Azure AD
4. **HTTPS certificate issues**: Trust the development certificate

### Debug Tips

- Enable detailed logging in `appsettings.Development.json`
- Use browser developer tools to inspect authentication flows
- Check Azure AD sign-in logs in the Azure Portal
- Verify JWT tokens at [jwt.io](https://jwt.io)

## License

This project is provided as an example for educational purposes.
