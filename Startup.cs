using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using SSOExampleApi.Services.Interfaces;
using SSOExampleApi.Services;
using SSOExampleApi.Configuration;

namespace SSOExampleApi;

/// <summary>
/// Application startup configuration class.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the Startup class.
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Gets the application configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures the services for the application.
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services">The service collection to configure</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Configure Azure AD authentication
        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();

        // Add JWT Bearer authentication for API endpoints
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

        // Configure authorization policies
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        // Add controllers with views for authentication flows
        services.AddControllersWithViews()
            .AddMicrosoftIdentityUI();

        // Add API controllers
        services.AddControllers();

        // Configure Swagger/OpenAPI
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "SSO Example API",
                Version = "v1",
                Description = "A sample API demonstrating Azure AD Single Sign-On integration"
            });

            // Configure JWT authentication in Swagger
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Register HTTP context accessor
        services.AddHttpContextAccessor();

        // Register application services
        RegisterApplicationServices(services);

        // Configure Azure AD options
        services.Configure<AzureAdOptions>(Configuration.GetSection("AzureAd"));
    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <param name="env">The web host environment</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "SSO Example API v1");
                options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }

    /// <summary>
    /// Registers application-specific services with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register services with</param>
    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
    }
}
