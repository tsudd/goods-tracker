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
                case nameof(CsvAdapter):
                    return new CsvAdapter(
                        adapterConfig,
                        loggerFactory.CreateLogger<CsvAdapter>());
                default:
                    throw new ArgumentException(
                        $"couldn't create {adapterConfig.AdapterName}: no such data adapter in the app"
                    );
            }
        }

        public static AdapterFactory GetSpecifiedFactory(string typeName)
        {
            try
            {
                return (AdapterFactory)(Type.GetType(typeName)?.GetMethod("GetInstance")?.Invoke(null, null)
                ?? throw new ArgumentException("No such adapter factory in the app"));
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
        }
    }
}