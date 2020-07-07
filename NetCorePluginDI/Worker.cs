using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCorePluginDI.PluginBase;

namespace NetCorePluginDI
{
    /// <summary>
    /// Based on the `dotnet new worker` template
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly List<IIntegration> _integrations;

        public Worker(IEnumerable<IIntegration> integrations)
        {
            _integrations = integrations.ToList();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = $"Worker running at: {DateTimeOffset.Now}";

                foreach (var integration in _integrations)
                {
                    await integration.ExecuteAsync(message, stoppingToken);
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
