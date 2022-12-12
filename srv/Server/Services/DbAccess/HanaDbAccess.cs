using Sap.Data.Hana;
using GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;
using System.Data.Common;

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
        _connection.Open();
    }

    public async Task<DbDataReader> ExecuteCommandAsync(string command)
    {
        using (var cmd = new HanaCommand(command, _connection))
        {
            try
            {
                return await cmd.ExecuteReaderAsync();
            }
            catch (HanaException ex)
            {
                _logger.LogError($"Error while communication (SELECT) with HANA: {ex.Message}");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }

    public void Dispose()
    {
        _connection?.Close();
    }

    public async Task<int> ExecuteNonQueryAsync(string command)
    {
        using (var cmd = new HanaCommand(command, _connection))
        {
            try
            {
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (HanaException ex)
            {
                _logger.LogError($"Error while communication (INSERT) with HANA: {ex.Message}");
                throw new InvalidOperationException(ex.Message);
            }
        }
    }
}