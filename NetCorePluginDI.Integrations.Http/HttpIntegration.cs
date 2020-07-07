using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCorePluginDI.PluginBase;

namespace NetCorePluginDI.Integrations.Http
{
    public class HttpIntegration : IIntegration
    {
        private readonly ILogger<HttpIntegration> _logger;
        private readonly HttpIntegrationSettings _settings;

        public HttpIntegration(ILogger<HttpIntegration> logger, HttpIntegrationSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        public string Name { get; } = "HTTP";
        public string Description { get; } = "An integration to send messages to a REST endpoint";
        public Task ExecuteAsync(string message, CancellationToken cancellationToken)
        {
            // Let's pretend we're sending messages to another service
            _logger.LogInformation($"Sending message [{message}] to [{_settings.Url}]");

            return Task.CompletedTask;
        }
    }
}
