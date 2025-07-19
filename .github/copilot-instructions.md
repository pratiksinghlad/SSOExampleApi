
# SSOExampleApi AI Coding Agent Instructions

## Big Picture Architecture
- .NET 9 Web API for Azure AD SSO using OAuth2/OpenID Connect, supporting both personal (MSA) and organizational accounts.
- Major components:
  - `Controllers/`: API endpoints (`AuthController`, `UserController`, etc.)
  - `Services/`: Business logic, split by domain, with interfaces in `Services/Interfaces/`
  - `Configuration/`: Strongly-typed config models (e.g., `AzureAdOptions.cs`)
  - `Models/`: DTOs for authentication, tokens, and user info
  - `Startup.cs`/`Startup.Swagger.cs`: App and Swagger/OAuth2 configuration
- Data flow: Auth requests go through Azure AD, tokens are validated, user info is extracted from claims, and APIs are protected by JWT Bearer authentication.

## Developer Workflows
- **Build/Run:** Use `dotnet run` from the repo root or the "Run SSO Example API" VS Code task.
- **Swagger UI:** Available at `https://localhost:5001`. OAuth2 login is integrated; click "Authorize" to sign in.
- **Azure AD Setup:** See `AZURE_SETUP.md` for registration, redirect URIs, and scopes. Update `appsettings.json` with your Azure AD details.
- **Configuration:** All secrets and keys are in `appsettings.json` and `appsettings.secrets.json`.
- **Testing:** Unit tests should be placed in a parallel `Tests` project (not present by default).

## Project-Specific Conventions
- All code targets .NET 9 and C# 13; use latest language features.
- Use dependency injection for all services; register in `Startup.cs`.
- Organize by feature/domain, not by layer.
- Interfaces go in `Services/Interfaces/`, implementations in feature folders.
- Use XML doc comments for all public APIs.
- Async methods must use `Task`/`Task<T>` and be suffixed with `Async`.
- Swagger OAuth2 is configured for Implicit flow by default, but Authorization Code flow with PKCE is recommended for React/SPAs.
  - See `Startup.Swagger.cs` for how OAuth2 endpoints are set.
- JWT validation is strict; issuer/audience must match Azure AD config.

## Integration Points
- **Azure AD:** All authentication flows use Azure AD endpoints. Personal accounts use the `consumers` tenant.
- **Swagger UI:** Integrated OAuth2 login; endpoints are protected and require Bearer tokens.
- **Microsoft Graph:** Scopes include `User.Read`, `openid`, `profile`, `email`.

## Examples
- To add a new service, create an interface in `Services/Interfaces/`, implement in `Services/`, and register in `Startup.cs`.
- To add a new endpoint, create a controller in `Controllers/` and decorate with `[Authorize]` as needed.
- To change OAuth2 flow, update `Startup.Swagger.cs` to use Authorization Code flow and PKCE.

## Key Files
- `Startup.cs`, `Startup.Swagger.cs`, `appsettings.json`, `Controllers/`, `Services/`, `Configuration/AzureAdOptions.cs`