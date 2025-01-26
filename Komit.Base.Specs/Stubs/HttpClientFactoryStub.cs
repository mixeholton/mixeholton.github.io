using Komit.Infrastructure.CqrsClient;

namespace Komit.Base.Specs.Stubs;
public record HttpClientFactoryStub(HttpClient Client) : IHttpClientFactoryAdaptor
{
    public HttpClient CreateClient(string name) => Client;
}
