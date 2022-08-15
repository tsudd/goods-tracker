namespace Common.Requesters;

public interface IRequester
{
    //TODO implement Uri class
    string Get(string url);
    string Post(string url, Dictionary<string, string> headers = null, string data = "");
    Task<string> PostAsync(string url, Dictionary<string, string> headers = null, string data = "");
    Task<string> Get(string url);
    HtmlDocument GetPageHtml(string url);
    Task<HtmlDocument> GetPageHtmlAsync(string url);
}