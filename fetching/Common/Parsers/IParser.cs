namespace Common.Parsers;

public interface IParser
{
    List<IEnumerable<string>> Parse(string rawItems);
}