# SSOExampleApi Solution

This repository contains a full-stack Single Sign-On (SSO) example using Azure Active Directory (Azure AD) with both a .NET 9 Web API backend and a React 19 + TypeScript frontend UI. It demonstrates secure authentication, token management, and modern development practices for enterprise and personal Microsoft accounts.

## Solution Structure

```
SSOExampleApi/
├── src/
│   ├── SSOExample.Api/      # .NET 9 Web API (Azure AD SSO, OAuth2, OpenID Connect)
│   └── UI/                  # React 19 + TypeScript SPA (MSAL, Azure AD SSO)
├── SSOExampleApi.sln        # Visual Studio solution file
├── README.md                # Solution-level documentation (this file)
```

## Key Features

- **Azure AD SSO**: Unified authentication for both API and UI
- **OAuth2 & OpenID Connect**: Secure flows with PKCE and JWT validation
- **Personal & Organizational Accounts**: Supports both Microsoft consumer and work accounts
- **Integrated Swagger UI**: API testing with OAuth2 login
- **React SPA**: Modern UI with MSAL authentication and automatic token refresh
- **Strict Security**: JWT validation, HTTPS, secure token storage

## Getting Started

1. **Clone the repository**
   ```cmd
   git clone https://github.com/<your-org>/SSOExampleApi.git
   cd SSOExampleApi
   ```
2. **Configure Azure AD**
   - Register your app in Azure Portal
   - Set up redirect URIs for both API and UI
   - Assign required Microsoft Graph scopes
   - See `src/SSOExample.Api/AZURE_SETUP.md` for details
3. **Update configuration**
   - Edit `src/SSOExample.Api/appsettings.json` with your Azure AD details
   - Edit `src/UI/src/config/authConfig.ts` for UI client settings
4. **Run the API**
   ```cmd
   cd src/SSOExample.Api
   dotnet run
   ```
5. **Run the UI**
   ```cmd
   cd ../UI
   npm install
   npm run dev
   ```
6. **Test SSO End-to-End**
   - Open `https://localhost:5001` for Swagger UI (API)
   - Open `http://localhost:5173` for React UI
   - Authenticate with your Microsoft account and test protected endpoints

## Folder Details

- `src/SSOExample.Api/` — .NET 9 Web API
  - Azure AD SSO, JWT validation, Swagger OAuth2
  - Controllers, Services, Models, Configuration
- `src/UI/` — React 19 + TypeScript SPA
  - MSAL authentication, token storage, API integration
  - Components, hooks, services, config

## Documentation

- API: See `src/SSOExample.Api/README.md` for backend details
- UI: See `src/UI/README.md` for frontend details
- Azure Setup: See `src/SSOExample.Api/AZURE_SETUP.md`
- Auth Implementation: See `src/UI/AUTH_IMPLEMENTATION.md`

## Prerequisites

- .NET 9 SDK
- Node.js 20+
- Azure AD tenant or personal Microsoft account

## Security Notes

- Store secrets securely (use Key Vault or environment variables in production)
- Use HTTPS for all endpoints
- Validate JWT tokens strictly (issuer, audience, expiration)

## License

This solution is provided for educational and development purposes.
