using System.Security.Claims;
using SSOExampleApi.Models;

namespace SSOExampleApi.Services.Interfaces;

/// <summary>
/// Interface for user-related operations and claims processing.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets the current user information from the claims principal.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal containing user information</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user information.</returns>
    Task<UserInfo> GetCurrentUserAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Gets user information by user ID.
    /// </summary>
    /// <param name="userId">The user ID to retrieve information for</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user information.</returns>
    Task<UserInfo?> GetUserByIdAsync(string userId);

    /// <summary>
    /// Validates if the user has the required permissions.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to validate</param>
    /// <param name="requiredPermissions">The required permissions</param>
    /// <returns>A task that represents the asynchronous operation. The task result indicates whether the user has the required permissions.</returns>
    Task<bool> HasPermissionsAsync(ClaimsPrincipal claimsPrincipal, params string[] requiredPermissions);
}
