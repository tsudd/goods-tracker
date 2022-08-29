using Microsoft.Extensions.Logging;

namespace Common.Parsers;

public class ParserFactory
{
    private static ParserFactory? _instance;

    private ParserFactory()
    {

    }

    public static ParserFactory GetInstance()
    {
        if (_instance is null)
        {
            _instance = new ParserFactory();
        }
        return _instance;
    }

    public IParser CreateParser(string parser, ILoggerFactory loggerFactory)
    {
        switch (parser)
        {
            case nameof(NeighborsParser):
                return new NeighborsParser(loggerFactory.CreateLogger<NeighborsParser>());
            default:
                throw new ArgumentException($"couldn't create {parser}: no such parser in the app.");
        }
    }

    public static ParserFactory GetSpecifiedFactory(string typeName)
    {
        try
        {
            return (ParserFactory)(Type.GetType(typeName)?.GetMethod("GetInstance")?.Invoke(null, null)
            ?? throw new ArgumentException("No such tracker factory in the app"));
        }
        catch (ArgumentException ex)
        {
            throw ex;
        }
    }
}