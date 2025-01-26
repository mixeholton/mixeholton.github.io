namespace Komit.SimpleApiAuth.TokenProvider
{
    public class TokenProviderOptions
    {
        public static TimeSpan DefaultTtl { get; set; } = TimeSpan.FromHours(12);
        public string Secret { get; set; } = Guid.Empty.ToString();
        public string Issuer { get; set; } = "identity.komit.nu";
        public string[] Audiences { get; set; } = ["Komit.Dev.Web"];
        public TimeSpan AccessTokenTtl { get; set; } = DefaultTtl;
    }
}
