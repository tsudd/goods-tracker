using Sap.Data.Hana;
using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;

namespace GoodsTracker.Platform.Server.Services.DbAccess;

internal sealed class HanaDbAccess : IDbAccess
{
    private const string CONNECTION_STRING_CONFIG = "HANA_ConnectionString";
    private readonly ILogger _logger;
    private readonly HanaConnection? _connection;

    public HanaDbAccess(ILogger<HanaDbAccess> logger, IConfiguration config)
    {
        _logger = logger;
        _connection = new HanaConnection(config[CONNECTION_STRING_CONFIG]);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}