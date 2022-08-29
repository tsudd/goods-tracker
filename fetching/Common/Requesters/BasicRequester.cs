using HtmlAgilityPack;
namespace Common.Requesters;

public class BasicRequester : IRequester
{
    public BasicRequester(HttpClient? client = null)
    {
        if (client is null)
        {
            Client = new HttpClient();
        }
        else
        {
            Client = client;
        }
    }

    public HttpClient Client { get; private set; }

    public async Task<HtmlDocument> GetPageHtmlAsync(string url)
    {
        var result = new HtmlDocument();
        var response = await Client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new HtmlWebException(
                $"couldn't get HTML page from {url}: {response.RequestMessage}");
        }
        result.LoadHtml(await response.Content.ReadAsStringAsync());
        return result;
    }

    public async Task<string> PostAsync(string url, Dictionary<string, string>? headers = null, string data = "")
    {
        var result = "";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        if ((headers != null))
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        request.Content = new StringContent(data);
        using (var response = await Client.SendAsync(request))
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HtmlWebException(
                    $"couldn't perform POST to {url}: {response.RequestMessage}");
            }
            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                result = await reader.ReadToEndAsync();
            }
        }
        return result;
    }

    public async Task<string> GetAsync(string url)
    {
        var result = "";
        using (var response = await Client.GetAsync(url))
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new HtmlWebException(
                    $"couldn't perform GET to {url}: {response.RequestMessage}");
            }
            using (var str = new StreamReader(await response.Content.ReadAsStreamAsync()))
            {
                result = await str.ReadToEndAsync();
            }
        }
        return result;
    }
}