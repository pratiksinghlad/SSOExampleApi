namespace SSOExampleApi.Models;

/// <summary>
/// Represents token information and metadata.
/// </summary>
public class TokenInfo
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID token.
    /// </summary>
    public string IdToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token type (usually "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Gets or sets the token expiration time.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the token scope.
    /// </summary>
    public string Scope { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional token properties.
    /// </summary>
    public Dictionary<string, string> AdditionalProperties { get; set; } = [];

    /// <summary>
    /// Gets a value indicating whether the token is expired.
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    /// <summary>
    /// Gets the time remaining until token expiration.
    /// </summary>
    public TimeSpan TimeUntilExpiration => ExpiresAt - DateTimeOffset.UtcNow;
}
