namespace SSOExampleApi.Models;

/// <summary>
/// Represents the result of an authentication operation.
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the authentication was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if authentication failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the redirect URL after authentication.
    /// </summary>
    public string RedirectUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional authentication properties.
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = [];

    /// <summary>
    /// Creates a successful authentication result.
    /// </summary>
    /// <param name="redirectUrl">The redirect URL</param>
    /// <returns>A successful authentication result</returns>
    public static AuthenticationResult Success(string redirectUrl = "")
    {
        return new AuthenticationResult
        {
            IsSuccess = true,
            RedirectUrl = redirectUrl
        };
    }

    /// <summary>
    /// Creates a failed authentication result.
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A failed authentication result</returns>
    public static AuthenticationResult Failure(string errorMessage)
    {
        return new AuthenticationResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
