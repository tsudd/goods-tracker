namespace Common.Requesters;

public class BasicRequester : IRequester
{
    public string Get(string url)
    {
        throw new NotImplementedException();
    }

    public HtmlDocument GetPageHtml(string url)
    {
        throw new NotImplementedException();
    }

    public Task<HtmlDocument> GetPageHtmlAsync(string url)
    {
        throw new NotImplementedException();
    }

    public string Post(string url, Dictionary<string, string> headers = null, string data = "")
    {
        throw new NotImplementedException();
    }

    public Task<string> PostAsync(string url, Dictionary<string, string> headers = null, string data = "")
    {
        throw new NotImplementedException();
    }

    Task<string> IRequester.Get(string url)
    {
        throw new NotImplementedException();
    }
}