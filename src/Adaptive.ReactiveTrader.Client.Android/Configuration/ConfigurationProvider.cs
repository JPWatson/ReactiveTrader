using Adaptive.ReactiveTrader.Client.Configuration;

namespace Adaptive.ReactiveTrader.Client.Android.Configuration
{
    internal sealed class ConfigurationProvider : IConfigurationProvider
    {
        public string[] Servers
        {
            //get { return new[] { "http://localhost:8080" }; }
            get { return new[] { "ws://130.211.60.124:8080/ws" }; }
        }
    }
}
