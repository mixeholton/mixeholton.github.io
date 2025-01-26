using Komit.Infrastructure.CqrsClient;
using System.Net.Http.Headers;

namespace Komit.Base.Dev.Client;

public class HttpClientFactoryUiAdaptor : IHttpClientFactoryAdaptor
{
    protected IHttpClientFactory ClientFactory { get; }
    protected SessionHandler Session { get; }
    public HttpClientFactoryUiAdaptor(IHttpClientFactory clientFactory, SessionHandler session)
    {
        ClientFactory = clientFactory;
        Session = session;
    }
    public HttpClient CreateClient(string name)
    {
        //To Do
        var client = ClientFactory.CreateClient(name);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Session.Token);
        return client;
    }
}
