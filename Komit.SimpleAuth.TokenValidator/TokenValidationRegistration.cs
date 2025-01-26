using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Komit.SimpleApiAuth.TokenValidator;

public static class TokenValidationRegistration
{
    public static IServiceCollection AddSimpleTokenValidation(this IServiceCollection services, TokenValidationOptions options)
    {
        services.AddAuthorization();
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            var key = Encoding.ASCII.GetBytes(options.Secret ?? throw new ArgumentException(nameof(TokenValidationOptions.Secret)));
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireExpirationTime = true,
                ValidateIssuer = !string.IsNullOrWhiteSpace(options.Issuer),
                ValidateAudience = options.Audiences.Any(),
                ValidIssuer = string.IsNullOrWhiteSpace(options.Issuer) ? null : options.Issuer,
                ValidAudiences = options.Audiences,
                ValidateLifetime = true
            };
            x.TokenValidationParameters = validationParameters;
        });
        return services;
    }
    public static IApplicationBuilder UseSimpleTokenValidation(this IApplicationBuilder app)
        => app.UseAuthentication().UseAuthorization();
}