using Komit.Infrastructure.CqrsClient;
using System.Net.Http.Headers;

namespace Komit.Base.Dev.Server;

public class HttpClientFactoryDevAdaptor : IHttpClientFactoryAdaptor
{
    protected IHttpClientFactory ClientFactory { get; }
    public string Token { get; set; }
    public HttpClientFactoryDevAdaptor(IHttpClientFactory clientFactory)
    {
        ClientFactory = clientFactory;
    }
    public HttpClient CreateClient(string name)
    {
        var client = ClientFactory.CreateClient(name);
        if (!string.IsNullOrWhiteSpace(Token))
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
        }
        return client;
    }
}
