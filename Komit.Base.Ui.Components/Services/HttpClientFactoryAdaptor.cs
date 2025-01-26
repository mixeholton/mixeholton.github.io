using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Ui.Components.Services;
public class HttpClientFactoryAdaptor : IHttpClientFactoryAdaptor
{
    protected IHttpClientFactory ClientFactory { get; }
    public HttpClientFactoryAdaptor(IHttpClientFactory clientFactory)
    {
        ClientFactory = clientFactory;
    }
    public HttpClient CreateClient(string name)
    {
        var client = ClientFactory.CreateClient(name);
        return client;
    }
}
