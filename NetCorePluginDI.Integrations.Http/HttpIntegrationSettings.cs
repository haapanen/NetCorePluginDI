using NetCorePluginDI.PluginBase;

namespace NetCorePluginDI.Integrations.Http
{
    public class HttpIntegrationSettings : ISettings
    {
        public string Url { get; set; }
    }
}