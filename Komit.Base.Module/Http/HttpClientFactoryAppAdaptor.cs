using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Module.Http;
public class HttpClientFactoryAppAdaptor : IHttpClientFactoryAdaptor
{
    protected IHttpClientFactory ClientFactory { get; }
    protected ISessionService Session { get; }
    public HttpClientFactoryAppAdaptor(IHttpClientFactory clientFactory, ISessionService session)
    {
        ClientFactory = clientFactory;
        Session = session;
    }
    public HttpClient CreateClient(string name)
    {
        var client = ClientFactory.CreateClient(name);
        client.DefaultRequestHeaders.Add("x_komit_session_id", Session.Value?.Id.ToString() ?? string.Empty);
        return client;
    }
}
