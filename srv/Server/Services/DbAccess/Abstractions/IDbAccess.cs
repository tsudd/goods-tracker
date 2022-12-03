using System.Data.Common;

namespace GoodsTracker.Platform.Server.Services.DbAccess.Abstractions;

public interface IDbAccess : IDisposable
{
    Task<DbDataReader> ExecuteCommandAsync(string command);
}