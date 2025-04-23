using FBMarketBot.IoC;
using FBMarketBot.Publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCore.AutoRegisterDi;
using NetEscapades.Extensions.Logging.RollingFile;

class Program
{
    /// <summary>
    /// Registers services for dependency injection.
    /// </summary>
    public static IServiceCollection ServicesRegistration()
    {
        var allAssemblies = SolutionAssemblies.GetAll();  // Get all assemblies from the solution.
        var filtredAssemblies = SolutionAssemblies.GetFiltred(allAssemblies);  // Filter the relevant assemblies.

        IServiceCollection services = new ServiceCollection();  // Create a new service collection.

        // Register transient dependencies from filtered assemblies.
        services.RegisterAssemblyPublicNonGenericClasses(filtredAssemblies.ToArray())
            .Where(typeOfClass => typeof(ITransientDependency).IsAssignableFrom(typeOfClass))
            .AsPublicImplementedInterfaces(ServiceLifetime.Transient);

        // Register singleton dependencies from filtered assemblies.
        services.RegisterAssemblyPublicNonGenericClasses(filtredAssemblies.ToArray())
            .Where(typeOfClass => typeof(ISingletonDependency).IsAssignableFrom(typeOfClass))
            .AsPublicImplementedInterfaces(ServiceLifetime.Singleton);

        // Configure logging services
        services.AddLogging(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "HH:mm:ss   ";  // Customize timestamp format for the console log.
            });

            // Configure file logging with specific options.
            builder.AddFile(options =>
            {
                options.LogDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");  // Set log directory.
                options.FileSizeLimit = 20 * 1024 * 1024;  // Set maximum log file size to 20MB.
                options.FilesPerPeriodicityLimit = 20;  // Set the number of log files before rolling over.
                options.Extension = "txt";  // Set the file extension for logs.
                options.Periodicity = PeriodicityOptions.Daily;  // Rotate logs daily.
            });
        });

        return services;
    }

    /// <summary>
    /// Entry point for the application.
    /// </summary>
    static async Task<int> Main(string[] args)
    {
        try
        {
            var services = ServicesRegistration();  // Register services.
            var serviceProvider = services.BuildServiceProvider();  // Build the service provider.

            // Retrieve and start the application from the DI container.
            var app = serviceProvider.GetRequiredService<IFaceBookAdPublisher>();
            await app.StartByScheduleAsync();  // Start the application.

            return 0;  // Return 0 on success.
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);  // Log exception if it occurs.
            return -1;  // Return -1 on error.
        }
    }
}
