using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSOExampleApi.Services.Interfaces;

namespace SSOExampleApi.Controllers;

/// <summary>
/// Controller for handling authentication operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authenticationService;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthController class.
    /// </summary>
    /// <param name="authenticationService">Authentication service</param>
    /// <param name="userService">User service</param>
    /// <param name="tokenService">Token service</param>
    /// <param name="logger">Logger instance</param>
    public AuthController(
        IAuthService authenticationService,
        IUserService userService,
        ITokenService tokenService,
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Initiates the authentication process with Azure AD.
    /// </summary>
    /// <param name="returnUrl">Optional return URL after authentication</param>
    /// <returns>Challenge result that redirects to Azure AD for authentication</returns>
    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        _logger.LogInformation("Login requested with return URL: {ReturnUrl}", returnUrl);
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(LoginCallback), "Auth", new { returnUrl })
        };

        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Handles the callback from Azure AD after authentication.
    /// </summary>
    /// <param name="returnUrl">Return URL after successful authentication</param>
    /// <returns>Redirect to the return URL or default page</returns>
    [HttpGet("login-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginCallback(string? returnUrl = null)
    {
        _logger.LogInformation("Login callback received");

        var isAuthenticated = await _authenticationService.IsAuthenticatedAsync();
        if (!isAuthenticated)
        {
            _logger.LogWarning("Authentication failed during callback");
            return BadRequest("Authentication failed");
        }

        var redirectUrl = returnUrl ?? "/api/user/profile";
        _logger.LogInformation("Authentication successful, redirecting to: {RedirectUrl}", redirectUrl);
        
        return Redirect(redirectUrl);
    }

    /// <summary>
    /// Signs out the current user from Azure AD.
    /// </summary>
    /// <param name="returnUrl">Optional return URL after sign-out</param>
    /// <returns>Sign-out result that redirects to Azure AD for sign-out</returns>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout(string? returnUrl = null)
    {
        _logger.LogInformation("Logout requested for user: {UserId}", User.Identity?.Name);

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? "/"
        };

        return SignOut(properties, 
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Gets the current authentication status.
    /// </summary>
    /// <returns>Authentication status information</returns>
    [HttpGet("status")]
    public async Task<IActionResult> GetAuthenticationStatus()
    {
        var isAuthenticated = await _authenticationService.IsAuthenticatedAsync();
        
        var status = new
        {
            IsAuthenticated = isAuthenticated,
            UserName = User.Identity?.Name,
            AuthenticationType = User.Identity?.AuthenticationType,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        };

        return Ok(status);
    }

    /// <summary>
    /// Validates a provided JWT token.
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>Token validation result</returns>
    [HttpPost("validate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("Token is required");
        }

        try
        {
            var claimsPrincipal = await _tokenService.ValidateTokenAsync(token);
            if (claimsPrincipal == null)
            {
                return Unauthorized("Invalid token");
            }

            var tokenInfo = await _tokenService.GetTokenInfoAsync(claimsPrincipal);
            var isExpired = await _tokenService.IsTokenExpiredAsync(token);

            var result = new
            {
                IsValid = true,
                IsExpired = isExpired,
                TokenInfo = tokenInfo,
                Claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return Unauthorized("Token validation failed");
        }
    }
}
