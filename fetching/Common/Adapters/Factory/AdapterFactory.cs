using Common.Configs;
using Microsoft.Extensions.Logging;

namespace Common.Adapters
{
    public class AdapterFactory
    {
        private static AdapterFactory? _instance;
        private AdapterFactory() { }

        public static AdapterFactory GetInstance()
        {
            if (_instance is null)
            {
                _instance = new AdapterFactory();
            }
            return _instance;
        }

        public IDataAdapter CreateAdapter(
            AdapterConfig adapterConfig,
            ILoggerFactory loggerFactory
        )
        {
            switch (adapterConfig.AdapterName)
            {
                case nameof(HanaAdapter):
                    return new HanaAdapter(
                        adapterConfig,
                        loggerFactory.CreateLogger<HanaAdapter>());
                default:
                    throw new ArgumentException(
                        $"couldn't create {adapterConfig.AdapterName}: no such data adapter in the app"
                    );
            }
        }
    }
}