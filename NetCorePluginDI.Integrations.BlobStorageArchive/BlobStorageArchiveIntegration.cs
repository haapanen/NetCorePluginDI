using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCorePluginDI.PluginBase;

namespace NetCorePluginDI.Integrations.BlobStorageArchive
{
    public class BlobStorageArchiveIntegration : IIntegration
    {
        private readonly ILogger<BlobStorageArchiveIntegration> _logger;

        public BlobStorageArchiveIntegration(ILogger<BlobStorageArchiveIntegration> logger)
        {
            _logger = logger;
        }

        public string Name { get; } = "Blob Storage Archive";
        public string Description { get; } = "An integration to archive messages to Blob Storage";
        public Task ExecuteAsync(string message, CancellationToken cancellationToken)
        {
            // Let's pretend we're archiving to Blob Storage
            _logger.LogInformation($"Archiving message [{message}] to Blob Storage");

            return Task.CompletedTask;
        }
    }
}
