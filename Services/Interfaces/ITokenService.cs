using System.Security.Claims;
using SSOExampleApi.Models;

namespace SSOExampleApi.Services.Interfaces;

/// <summary>
/// Interface for token-related operations and JWT handling.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Validates a JWT token and returns the claims principal.
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validated claims principal.</returns>
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);

    /// <summary>
    /// Gets token information from the current request context.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal containing token information</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the token information.</returns>
    Task<TokenInfo> GetTokenInfoAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new token information.</returns>
    Task<TokenInfo?> RefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Checks if a token is expired.
    /// </summary>
    /// <param name="token">The token to check</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the token is expired.</returns>
    Task<bool> IsTokenExpiredAsync(string token);
}
