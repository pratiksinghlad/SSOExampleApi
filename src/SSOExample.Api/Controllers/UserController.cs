using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSOExampleApi.Services.Interfaces;

namespace SSOExampleApi.Controllers;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Initializes a new instance of the UserController class.
    /// </summary>
    /// <param name="userService">User service</param>
    /// <param name="tokenService">Token service</param>
    /// <param name="logger">Logger instance</param>
    public UserController(
        IUserService userService,
        ITokenService tokenService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authenticated user's profile information.
    /// </summary>
    /// <returns>Current user's profile information</returns>
    [HttpGet("profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        try
        {
            var userInfo = await _userService.GetCurrentUserAsync(User);
            _logger.LogInformation("Retrieved profile for user: {UserId}", userInfo.Id);
            
            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            return StatusCode(500, "Error retrieving user profile");
        }
    }

    /// <summary>
    /// Gets user information by user ID.
    /// </summary>
    /// <param name="userId">The user ID to retrieve information for</param>
    /// <returns>User information for the specified ID</returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return BadRequest("User ID is required");
        }

        try
        {
            var userInfo = await _userService.GetUserByIdAsync(userId);
            if (userInfo == null)
            {
                return NotFound($"User with ID '{userId}' not found");
            }

            _logger.LogInformation("Retrieved user information for ID: {UserId}", userId);
            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {UserId}", userId);
            return StatusCode(500, "Error retrieving user information");
        }
    }

    /// <summary>
    /// Gets the current user's token information.
    /// </summary>
    /// <returns>Token information for the current user</returns>
    [HttpGet("token-info")]
    public async Task<IActionResult> GetTokenInfo()
    {
        try
        {
            var tokenInfo = await _tokenService.GetTokenInfoAsync(User);
            _logger.LogInformation("Retrieved token information for user");
            
            return Ok(tokenInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving token information");
            return StatusCode(500, "Error retrieving token information");
        }
    }

    /// <summary>
    /// Checks if the current user has the specified permissions.
    /// </summary>
    /// <param name="permissions">Array of permissions to check</param>
    /// <returns>Permission check result</returns>
    [HttpPost("check-permissions")]
    public async Task<IActionResult> CheckPermissions([FromBody] string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
        {
            return BadRequest("Permissions array is required");
        }

        try
        {
            var hasPermissions = await _userService.HasPermissionsAsync(User, permissions);
            
            var result = new
            {
                HasPermissions = hasPermissions,
                RequestedPermissions = permissions,
                UserId = User.Identity?.Name
            };

            _logger.LogInformation("Permission check completed for user. Has permissions: {HasPermissions}", hasPermissions);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permissions");
            return StatusCode(500, "Error checking permissions");
        }
    }

    /// <summary>
    /// Gets all claims for the current authenticated user.
    /// </summary>
    /// <returns>List of user claims</returns>
    [HttpGet("claims")]
    public IActionResult GetUserClaims()
    {
        try
        {
            var claims = User.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value,
                ValueType = c.ValueType,
                Issuer = c.Issuer
            }).ToList();

            _logger.LogInformation("Retrieved {ClaimCount} claims for user", claims.Count);
            return Ok(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user claims");
            return StatusCode(500, "Error retrieving user claims");
        }
    }

    /// <summary>
    /// Gets user roles from claims.
    /// </summary>
    /// <returns>List of user roles</returns>
    [HttpGet("roles")]
    public async Task<IActionResult> GetUserRoles()
    {
        try
        {
            var userInfo = await _userService.GetCurrentUserAsync(User);
            
            var result = new
            {
                UserId = userInfo.Id,
                UserName = userInfo.DisplayName,
                Roles = userInfo.Roles,
                Groups = userInfo.Groups
            };

            _logger.LogInformation("Retrieved roles for user: {UserId}", userInfo.Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user roles");
            return StatusCode(500, "Error retrieving user roles");
        }
    }
}
