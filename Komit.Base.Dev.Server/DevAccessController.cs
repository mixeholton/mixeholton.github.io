using Komit.Base.Values.Cqrs;
using Komit.SimpleApiAuth.TokenProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Komit.Base.Dev.Server;

[ApiController]
[Route("Api/[controller]/[action]")]
public class DevAccessController : ControllerBase
{
    protected IConfiguration Configuration { get; }
    protected IDistributedCache SessionCache { get; }
    public DevAccessController(IConfiguration configuration, IDistributedCache sessionCache)
    {
        Configuration = configuration;
        SessionCache = sessionCache;
    }
    [HttpGet("{sessionId}")]
    public IActionResult GetToken([FromServices] TokenProvider tokenProvider, [FromRoute] Guid sessionId)
    {
        if (sessionId == null || sessionId == default)
            return BadRequest();
        var claims = new Dictionary<string, string>
            {
                { "extension_SessionId", sessionId.ToString() }
            };


            var sessionString = SessionCache.GetString(sessionId.ToString());
            var session = string.IsNullOrWhiteSpace(sessionString)
                ? SessionValue.NewAnonymous() with { Id = sessionId }
                : JsonSerializer.Deserialize<SessionValue>(sessionString);
        if (session?.IsAnonymous() == true && Configuration.GetRequiredSection("DefaultSession").GetValue<bool>("Use"))
        {
            var userId = Configuration.GetRequiredSection("DefaultSession").GetValue<Guid>("UserId");
            var tenantId = Configuration.GetRequiredSection("DefaultSession").GetValue<int>("TenantId");
            var affiliationId = Configuration.GetRequiredSection("DefaultSession").GetValue<Guid?>("AffiliationId") ?? Guid.NewGuid();
            var systemName = Configuration.GetRequiredSection("DefaultSession").GetValue<string>("SystemName");
            var permissions = Configuration.GetRequiredSection("DefaultSession").GetValue<string[]>("Permissions") ?? [];
            session = new SessionValue(session.Id, 1, userId, tenantId, affiliationId, [new("DB", systemName)], permissions);
            SessionCache.SetString(session.Id.ToString(), JsonSerializer.Serialize(session), new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromHours(8)
            });
        }
        return Ok(tokenProvider.GenerateToken(claims));
    }
    [HttpGet]
    public IActionResult GetSession()
    {
        var sessionId = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "extension_SessionId")?.Value
            ?? HttpContext.Request.Headers.Referer.ToString().Split("?session=").Last();
        SessionValue session = null;
        if (Guid.TryParse(sessionId, out var sessionGuid))
        {
            var sessionString = SessionCache.GetString(sessionId.ToString());
            session = string.IsNullOrWhiteSpace(sessionString)
                ? SessionValue.NewAnonymous() with { Id = sessionGuid }
                : JsonSerializer.Deserialize<SessionValue>(sessionString);
        }
        return Ok(session ?? SessionValue.NewAnonymous());
    }
    [HttpGet]
    public IActionResult GetConfiguration()
    {
        return Ok(Configuration.AsEnumerable());
    }
}
