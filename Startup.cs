using Microsoft.AspNetCore.Authentication.JwtBearer;
using SSOExampleApi.Configuration;
using SSOExampleApi.Services;
using SSOExampleApi.Services.Interfaces;

namespace SSOExampleApi;

/// <summary>
/// Application startup configuration class.
/// </summary>
public partial class Startup
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
        // Configure Azure AD authentication for Web API with personal accounts
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://login.microsoftonline.com/consumers/v2.0";
                options.Audience = Configuration["AzureAd:ClientId"];
                
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://login.microsoftonline.com/9188040d-6c67-4c5b-b112-36a304b66dad/v2.0",
                    ValidateAudience = true,
                    ValidAudiences = new[] 
                    { 
                        Configuration["AzureAd:ClientId"],
                        $"api://{Configuration["AzureAd:ClientId"]}"
                    },
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogError("Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogInformation("Token validated for user: {User}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });

        // Configure authorization policies
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        // Add API controllers
        services.AddControllers();

        // Configure Swagger/OpenAPI with OAuth2
        ConfigureSwagger(services);

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
            // Configure Swagger before authentication
            ConfigureSwaggerUI(app, env);
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
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
