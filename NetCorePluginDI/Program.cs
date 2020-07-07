using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCorePluginDI.PluginBase;

namespace NetCorePluginDI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
                    var settings = new NetCorePluginDISettings();
                    configuration.Bind(settings);
                    services.AddSingleton(settings);

                    services.AddHostedService<Worker>();

                    RegisterIntegrations(services, configuration, settings);
                });

        private static void RegisterIntegrations(IServiceCollection services, IConfiguration configuration,
            NetCorePluginDISettings settings)
        {
            foreach (var integrationPath in settings.IntegrationPaths)
            {
                var assembly = LoadIntegrationAssembly(integrationPath);
                RegisterIntegrationsFromAssembly(services, configuration, assembly);
            }
        }

        /// <summary>
        /// Based on https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support#load-plugins
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private static Assembly LoadIntegrationAssembly(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading integrations from: {pluginLocation}");
            var loadContext = new IntegrationLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        /// <summary>
        /// Based on https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support#create-the-plugin-interfaces
        /// with alterations to support injecting settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assembly"></param>
        private static void RegisterIntegrationsFromAssembly(IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                // Register all classes that implement the IIntegration interface
                if (typeof(IIntegration).IsAssignableFrom(type))
                {
                    // Add as a singleton as the Worker is a singleton and we'll only have one
                    // instance. If this would be a Controller or something else with clearly defined
                    // scope that is not the lifetime of the application, use AddScoped.
                    services.AddSingleton(typeof(IIntegration), type);
                }

                // Register all classes that implement the ISettings interface
                if (typeof(ISettings).IsAssignableFrom(type))
                {
                    var settings = Activator.CreateInstance(type);
                    // appsettings.json or some other configuration provider should contain
                    // a key with the same name as the type
                    // e.g. "HttpIntegrationSettings": { ... }
                    if (!configuration.GetSection(type.Name).Exists())
                    {
                        // If it does not contain the key, throw an error
                        throw new ArgumentException($"Configuration does not contain key [{type.Name}]");
                    }
                    configuration.Bind(type.Name, settings);

                    // Settings can be singleton as we'll only ever read it
                    services.AddSingleton(type, settings);
                }
            }
        }
    }
}
