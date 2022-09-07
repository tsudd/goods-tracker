using Sap.Data.Hana;
using Shared.Model;
using Server.DbOptions;

namespace Server.Services;
public class HanaSetup : IDbSetup
{
    private ILogger _logger;
    private DbContext _dbContext;


    public HanaSetup(ILogger<HanaSetup> logger, DbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public Task<IEnumerable<Item>> GetIemsAsync(DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Item>> GetItemsAsync()
    {
        throw new NotImplementedException();
    }

    public Task SetupDbAsync()
    {
        throw new NotImplementedException();
    }

    public string TestMethod()
    {
        return _dbContext.ConnectionString;
    }
}
