using HtmlAgilityPack;
namespace Common.Requesters;

public interface IRequester
{
    Task<string> PostAsync(string url, Dictionary<string, string>? headers = null, string data = "");
    Task<string> GetAsync(string url);
    Task<HtmlDocument> GetPageHtmlAsync(string url);
}