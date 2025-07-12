using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SSOExampleApi.Models;
using SSOExampleApi.Services.Interfaces;

namespace SSOExampleApi.Services;

/// <summary>
/// Service for managing authentication-related operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthService class.
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor</param>
    /// <param name="logger">Logger instance</param>
    public AuthService(IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Initiates the Azure AD authentication flow.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after authentication</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication challenge result.</returns>
    public Task<AuthenticationResult> ChallengeAsync(string? returnUrl = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Task.FromResult(AuthenticationResult.Failure("HTTP context not available"));
            }

            var properties = new AuthenticationProperties();
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                properties.RedirectUri = returnUrl;
            }

            // In a real controller, this would be:
            // return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
            _logger.LogInformation("Authentication challenge initiated for return URL: {ReturnUrl}", returnUrl);
            
            return Task.FromResult(AuthenticationResult.Success(returnUrl ?? "/"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating authentication challenge");
            return Task.FromResult(AuthenticationResult.Failure("Authentication challenge failed"));
        }
    }

    /// <summary>
    /// Signs out the current user from Azure AD.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after sign-out</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the sign-out result.</returns>
    public Task<AuthenticationResult> SignOutAsync(string? returnUrl = null)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Task.FromResult(AuthenticationResult.Failure("HTTP context not available"));
            }

            var properties = new AuthenticationProperties();
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                properties.RedirectUri = returnUrl;
            }

            // In a real controller, this would be:
            // return SignOut(properties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            _logger.LogInformation("Sign-out initiated for return URL: {ReturnUrl}", returnUrl);
            
            return Task.FromResult(AuthenticationResult.Success(returnUrl ?? "/"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating sign-out");
            return Task.FromResult(AuthenticationResult.Failure("Sign-out failed"));
        }
    }

    /// <summary>
    /// Gets the authentication properties for the current request.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication properties.</returns>
    public async Task<AuthenticationProperties?> GetAuthenticationPropertiesAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        try
        {
            var authenticateResult = await httpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return authenticateResult.Properties;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication properties");
            return null;
        }
    }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the user is authenticated.</returns>
    public Task<bool> IsAuthenticatedAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User == null)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(httpContext.User.Identity?.IsAuthenticated ?? false);
    }
}
