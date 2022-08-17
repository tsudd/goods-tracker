using Microsoft.Extensions.Logging;

namespace Common.Parsers;

public class NeighborsParser : IParser
{
    private ILogger<NeighborsParser> _logger;
    public NeighborsParser(ILogger<NeighborsParser> logger)
    {
        _logger = logger;
    }

    public List<IEnumerable<string>> Parse(string rawItems)
    {
        throw new NotImplementedException();
    }
}