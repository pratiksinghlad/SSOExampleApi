using SSOExampleApi;

namespace SSOExampleApi;

/// <summary>
/// The main entry point for the SSO Example API application.
/// </summary>
public class Program
{
    /// <summary>
    /// Main application entry point.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Creates and configures the host builder for the application.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Configured IHostBuilder instance</returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
