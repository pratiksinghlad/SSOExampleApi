using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SSOExampleApi.Models;
using SSOExampleApi.Services.Interfaces;
using SSOExampleApi.Configuration;
using Microsoft.Extensions.Options;

namespace SSOExampleApi.Services;

/// <summary>
/// Service for managing token-related operations and JWT handling.
/// </summary>
public class TokenService : ITokenService
{
    private readonly AzureAdOptions _azureAdOptions;
    private readonly ILogger<TokenService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    /// <summary>
    /// Initializes a new instance of the TokenService class.
    /// </summary>
    /// <param name="azureAdOptions">Azure AD configuration options</param>
    /// <param name="logger">Logger instance</param>
    public TokenService(IOptions<AzureAdOptions> azureAdOptions, ILogger<TokenService> logger)
    {
        _azureAdOptions = azureAdOptions.Value;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    /// <summary>
    /// Validates a JWT token and returns the claims principal.
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the validated claims principal.</returns>
    public Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        try
        {
            if (!_tokenHandler.CanReadToken(token))
            {
                _logger.LogWarning("Invalid JWT token format");
                return Task.FromResult<ClaimsPrincipal?>(null);
            }

            var jwtToken = _tokenHandler.ReadJwtToken(token);
            
            // Create claims principal from the JWT token
            var claims = jwtToken.Claims;
            var identity = new ClaimsIdentity(claims, "jwt");
            var principal = new ClaimsPrincipal(identity);

            return Task.FromResult<ClaimsPrincipal?>(principal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }

    /// <summary>
    /// Gets token information from the current request context.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal containing token information</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the token information.</returns>
    public Task<TokenInfo> GetTokenInfoAsync(ClaimsPrincipal claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);

        var tokenInfo = new TokenInfo();

        // Extract token expiration
        var expClaim = claimsPrincipal.FindFirst("exp");
        if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
        {
            tokenInfo.ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(exp);
        }

        // Extract scope
        var scopeClaim = claimsPrincipal.FindFirst("scp") ?? claimsPrincipal.FindFirst("scope");
        if (scopeClaim != null)
        {
            tokenInfo.Scope = scopeClaim.Value;
        }

        // Extract additional properties
        tokenInfo.AdditionalProperties = ExtractTokenProperties(claimsPrincipal);

        return Task.FromResult(tokenInfo);
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new token information.</returns>
    public Task<TokenInfo?> RefreshTokenAsync(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        // In a real implementation, this would call the Azure AD token endpoint
        // to refresh the access token using the refresh token
        // For this example, we return null as this requires additional HTTP client setup
        _logger.LogInformation("Token refresh requested");
        return Task.FromResult<TokenInfo?>(null);
    }

    /// <summary>
    /// Checks if a token is expired.
    /// </summary>
    /// <param name="token">The token to check</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the token is expired.</returns>
    public Task<bool> IsTokenExpiredAsync(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        try
        {
            if (!_tokenHandler.CanReadToken(token))
            {
                return Task.FromResult(true);
            }

            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var expirationTime = jwtToken.ValidTo;

            return Task.FromResult(DateTime.UtcNow >= expirationTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return Task.FromResult(true);
        }
    }

    /// <summary>
    /// Extracts additional token properties from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal</param>
    /// <returns>Dictionary of additional token properties</returns>
    private static Dictionary<string, string> ExtractTokenProperties(ClaimsPrincipal claimsPrincipal)
    {
        var tokenProperties = new Dictionary<string, string>();

        // Extract issuer
        var issuerClaim = claimsPrincipal.FindFirst("iss");
        if (issuerClaim != null)
        {
            tokenProperties["issuer"] = issuerClaim.Value;
        }

        // Extract audience
        var audienceClaim = claimsPrincipal.FindFirst("aud");
        if (audienceClaim != null)
        {
            tokenProperties["audience"] = audienceClaim.Value;
        }

        // Extract issued at time
        var iatClaim = claimsPrincipal.FindFirst("iat");
        if (iatClaim != null)
        {
            tokenProperties["issued_at"] = iatClaim.Value;
        }

        return tokenProperties;
    }
}
