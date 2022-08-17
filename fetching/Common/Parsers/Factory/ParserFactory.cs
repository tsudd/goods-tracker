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
                throw new ArgumentException("No such parser in the app.");
        }
    }

}