using GoodsTracker.DataCollector.Common.Configs;
using GoodsTracker.DataCollector.Common.Factories.Implementations;
using Microsoft.Extensions.Logging;

namespace GoodsTracker.DataCollector.Tests;

public class HanaAdapterTests
{
    [Fact]
    public void WhenInsertingShopItems()
    {
        var factory = DataCollectorFactory.GetInstance();
        var config = new AdapterConfig
        {
            AdapterName = "HanaAdapter",
            LocalPath = "./../../../FetchedItems",
            Arguments = "Server=9bc5e56d-aba0-4d2a-b7ca-818f4c41e0d3.hana.trial-us10.hanacloud.ondemand.com:443;UID=98475DBA342640B8BE326E8B25F3F830_ALWUS2H75Y3ZAZIH06VSPDQ5B_RT;PWD=Cp5cA-7f6SFViSm6uUd7rFMGWToq6RUAGgs4vUm72FFWe0isIf2gNcGwO7UNIX7gceRZ-zW05yLDYT4PuaoPmzIztXMydaOCGxC6qtxlSeKENTvgiZFLpo26t5ex1Xca;encrypt=true;sslValidateCertificate=true;Current Schema=98475DBA342640B8BE326E8B25F3F830",
        };
        // var adpater = factory.CreateDataAdapter(config, LoggerFactory.Create(LoggerFactory.Create(builder => builder.Configure())
        //     .CreateLogger())
    }
}