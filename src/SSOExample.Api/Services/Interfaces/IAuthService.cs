using Microsoft.AspNetCore.Authentication;
using SSOExampleApi.Models;

namespace SSOExampleApi.Services.Interfaces;

/// <summary>
/// Interface for authentication-related operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Initiates the Azure AD authentication flow.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after authentication</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication challenge result.</returns>
    Task<AuthenticationResult> ChallengeAsync(string? returnUrl = null);

    /// <summary>
    /// Signs out the current user from Azure AD.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after sign-out</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the sign-out result.</returns>
    Task<AuthenticationResult> SignOutAsync(string? returnUrl = null);

    /// <summary>
    /// Gets the authentication properties for the current request.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication properties.</returns>
    Task<AuthenticationProperties?> GetAuthenticationPropertiesAsync();

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the user is authenticated.</returns>
    Task<bool> IsAuthenticatedAsync();
}
