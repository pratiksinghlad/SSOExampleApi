using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SSOExampleApi.Controllers;

/// <summary>
/// Home controller for basic application endpoints.
/// </summary>
[Route("")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the HomeController class.
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Default endpoint that provides API information.
    /// </summary>
    /// <returns>API information</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        var apiInfo = new
        {
            Name = "SSO Example API",
            Version = "1.0.0",
            Description = "A sample API demonstrating Azure AD Single Sign-On integration",
            Endpoints = new
            {
                Authentication = new
                {
                    Login = "/api/auth/login",
                    Logout = "/api/auth/logout",
                    Status = "/api/auth/status",
                    ValidateToken = "/api/auth/validate-token"
                },
                User = new
                {
                    Profile = "/api/user/profile",
                    TokenInfo = "/api/user/token-info",
                    Claims = "/api/user/claims",
                    Roles = "/api/user/roles",
                    CheckPermissions = "/api/user/check-permissions"
                },
                Documentation = new
                {
                    Swagger = "/swagger",
                    OpenApi = "/swagger/v1/swagger.json"
                }
            },
            Authentication = new
            {
                Type = "Azure AD OAuth 2.0 / OpenID Connect",
                RequiredScopes = new[] { "openid", "profile", "email" },
                SupportedAccountTypes = "Personal Microsoft accounts (including Outlook.com, Live.com)"
            }
        };

        return Ok(apiInfo);
    }

    /// <summary>
    /// Health check endpoint.
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        var health = new
        {
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        _logger.LogInformation("Health check requested");
        return Ok(health);
    }

    /// <summary>
    /// Error handling endpoint.
    /// </summary>
    /// <returns>Error information</returns>
    [HttpGet("error")]
    [AllowAnonymous]
    public IActionResult Error()
    {
        var error = new
        {
            Error = "An error occurred",
            Message = "Please check the logs for more information",
            Timestamp = DateTimeOffset.UtcNow
        };

        _logger.LogError("Error endpoint accessed");
        return StatusCode(500, error);
    }
}
