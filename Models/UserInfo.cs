namespace SSOExampleApi.Models;

/// <summary>
/// Represents user information retrieved from Azure AD claims.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// Gets or sets the unique user identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's given name (first name).
    /// </summary>
    public string GivenName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's surname (last name).
    /// </summary>
    public string Surname { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's job title.
    /// </summary>
    public string JobTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's roles.
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = [];

    /// <summary>
    /// Gets or sets the user's groups.
    /// </summary>
    public IEnumerable<string> Groups { get; set; } = [];

    /// <summary>
    /// Gets or sets the tenant ID the user belongs to.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional user claims.
    /// </summary>
    public Dictionary<string, string> AdditionalClaims { get; set; } = [];
}
