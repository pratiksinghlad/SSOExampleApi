using System.Security.Claims;
using SSOExampleApi.Models;
using SSOExampleApi.Services.Interfaces;

namespace SSOExampleApi.Services;

/// <summary>
/// Service for managing user-related operations and claims processing.
/// </summary>
public class UserService : IUserService
{
    /// <summary>
    /// Gets the current user information from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal containing user information</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user information.</returns>
    public Task<UserInfo> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);

        var userInfo = new UserInfo
        {
            Id = GetClaimValue(claimsPrincipal, ClaimTypes.NameIdentifier) ?? 
                 GetClaimValue(claimsPrincipal, "oid") ?? string.Empty,
            Email = GetClaimValue(claimsPrincipal, ClaimTypes.Email) ?? 
                    GetClaimValue(claimsPrincipal, "preferred_username") ?? string.Empty,
            DisplayName = GetClaimValue(claimsPrincipal, ClaimTypes.Name) ?? 
                         GetClaimValue(claimsPrincipal, "name") ?? string.Empty,
            GivenName = GetClaimValue(claimsPrincipal, ClaimTypes.GivenName) ?? 
                        GetClaimValue(claimsPrincipal, "given_name") ?? string.Empty,
            Surname = GetClaimValue(claimsPrincipal, ClaimTypes.Surname) ?? 
                      GetClaimValue(claimsPrincipal, "family_name") ?? string.Empty,
            JobTitle = GetClaimValue(claimsPrincipal, "jobTitle") ?? string.Empty,
            TenantId = GetClaimValue(claimsPrincipal, "tid") ?? string.Empty,
            Roles = GetClaimValues(claimsPrincipal, ClaimTypes.Role),
            Groups = GetClaimValues(claimsPrincipal, "groups"),
            AdditionalClaims = ExtractAdditionalClaims(claimsPrincipal)
        };

        return Task.FromResult(userInfo);
    }

    /// <summary>
    /// Gets user information by user ID.
    /// </summary>
    /// <param name="userId">The user ID to retrieve information for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user information.</returns>
    public Task<UserInfo?> GetUserByIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        // In a real implementation, this would query a database or external service
        // For this example, we return null as this would require additional setup
        return Task.FromResult<UserInfo?>(null);
    }

    /// <summary>
    /// Validates if the user has the required permissions.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to validate</param>
    /// <param name="requiredPermissions">The required permissions</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the user has the required permissions.</returns>
    public Task<bool> HasPermissionsAsync(ClaimsPrincipal claimsPrincipal, params string[] requiredPermissions)
    {
        ArgumentNullException.ThrowIfNull(claimsPrincipal);
        ArgumentNullException.ThrowIfNull(requiredPermissions);

        if (requiredPermissions.Length == 0)
        {
            return Task.FromResult(true);
        }

        var userRoles = GetClaimValues(claimsPrincipal, ClaimTypes.Role).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var hasPermissions = requiredPermissions.Any(permission => userRoles.Contains(permission));

        return Task.FromResult(hasPermissions);
    }

    /// <summary>
    /// Gets a single claim value from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal</param>
    /// <param name="claimType">The claim type to retrieve</param>
    /// <returns>The claim value or null if not found</returns>
    private static string? GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return claimsPrincipal.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Gets all claim values for a specific claim type from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal</param>
    /// <param name="claimType">The claim type to retrieve</param>
    /// <returns>Collection of claim values</returns>
    private static IEnumerable<string> GetClaimValues(ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return claimsPrincipal.FindAll(claimType).Select(c => c.Value);
    }

    /// <summary>
    /// Extracts additional claims from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal</param>
    /// <returns>Dictionary of additional claims</returns>
    private static Dictionary<string, string> ExtractAdditionalClaims(ClaimsPrincipal claimsPrincipal)
    {
        var standardClaims = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ClaimTypes.NameIdentifier, ClaimTypes.Email, ClaimTypes.Name,
            ClaimTypes.GivenName, ClaimTypes.Surname, ClaimTypes.Role,
            "oid", "preferred_username", "name", "given_name", "family_name",
            "jobTitle", "tid", "groups"
        };

        return claimsPrincipal.Claims
            .Where(c => !standardClaims.Contains(c.Type))
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(c => c.Value)));
    }
}
