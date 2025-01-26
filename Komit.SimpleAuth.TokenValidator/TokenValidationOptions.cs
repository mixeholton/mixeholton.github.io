namespace Komit.SimpleApiAuth.TokenValidator;

public class TokenValidationOptions
{
    public string Issuer { get; set; } = "identity.komit.nu";
    public string[] Audiences { get; set; } = ["Komit.Dev.Web"];
    public string Secret { get; set; } = Guid.Empty.ToString();
}