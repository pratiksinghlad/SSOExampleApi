using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SSOExampleApi;

/// <summary>
/// Partial class containing Swagger configuration methods.
/// </summary>
public partial class Startup
{
    /// <summary>
    /// Configures Swagger/OpenAPI services with OAuth2 authentication.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Custom schema IDs to solve naming conflicts
            options.CustomSchemaIds(type => type.ToString());

            options.SwaggerDoc("v1", new()
            {
                Title = "SSO Example API",
                Version = "v1",
                Description = "A sample API demonstrating Azure AD Single Sign-On integration",
                Contact = new() { Email = "pratiklad9625@gmail.com" }
            });

            // Configure OAuth2 authentication with Azure AD
            var tenantId = Configuration["AzureAd:TenantId"];
            var isPersonalAccounts = tenantId?.Equals("consumers", StringComparison.OrdinalIgnoreCase) ?? false;

            var authUrl = isPersonalAccounts 
                ? $"{Configuration["AzureAd:Instance"]}consumers/oauth2/v2.0/authorize"
                : $"{Configuration["AzureAd:Instance"]}{tenantId}/oauth2/v2.0/authorize";
                
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authUrl),
                        Scopes = new Dictionary<string, string> 
                        { 
                            { $"api://{Configuration["AzureAd:ClientId"]}/access_as_user", "Access API as user" },
                            { "openid", "Sign in permissions" },
                            { "profile", "View your basic profile" },
                            { "email", "View your email address" }
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme, 
                            Id = "oauth2"
                        },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });

            // Include XML documentation if available
            var xmlDocumentation = Path.Combine(AppContext.BaseDirectory, "SSOExampleApi.xml");
            if (File.Exists(xmlDocumentation))
            {
                options.IncludeXmlComments(xmlDocumentation);
            }
        });
    }

    /// <summary>
    /// Configures Swagger UI with OAuth2 settings.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="env">The web host environment</param>
    private void ConfigureSwaggerUI(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "SSO Example API v1");
            
            // Configure for Implicit flow
            options.OAuthClientId(Configuration["AzureAd:ClientId"]);
            options.EnablePersistAuthorization();

            // Collapse sections for a cleaner view
            options.DocExpansion(DocExpansion.None);
        });
    }
}
