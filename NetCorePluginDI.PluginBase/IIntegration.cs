using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCorePluginDI.PluginBase
{
    /// <summary>
    /// Based on example at https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
    /// </summary>
    public interface IIntegration
    {
        string Name { get; }
        string Description { get; }
        Task ExecuteAsync(string message, CancellationToken cancellationToken);
    }
}
