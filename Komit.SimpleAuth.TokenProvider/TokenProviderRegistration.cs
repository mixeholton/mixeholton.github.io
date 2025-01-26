using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Komit.SimpleApiAuth.TokenProvider;

public static class TokenProviderRegistration
{
    public static IServiceCollection AddSimpleTokenProvider(this IServiceCollection services, TokenProviderOptions options)
        => services.AddSingleton(x => new TokenProvider(options));
}