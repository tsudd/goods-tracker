namespace Server.DbOptions;

public class DbContext
{
    public string ConnectionString { get; private set; } = String.Empty;

    public DbContext(string connectionString)
    {
        ConnectionString = connectionString;
    }
}