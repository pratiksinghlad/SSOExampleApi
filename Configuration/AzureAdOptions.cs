namespace SSOExampleApi.Configuration;

/// <summary>
/// Configuration options for Azure Active Directory integration.
/// </summary>
public class AzureAdOptions
{
    /// <summary>
    /// Gets or sets the Azure AD instance URL (e.g., https://login.microsoftonline.com/).
    /// </summary>
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Azure AD domain (e.g., contoso.onmicrosoft.com).
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Azure AD tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client ID (Application ID) of the registered application.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret of the registered application.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the callback path for authentication responses.
    /// </summary>
    public string CallbackPath { get; set; } = "/signin-oidc";

    /// <summary>
    /// Gets or sets the sign-out callback path.
    /// </summary>
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";

    /// <summary>
    /// Gets or sets the sign-out URL.
    /// </summary>
    public string SignOutUrl { get; set; } = string.Empty;
}
