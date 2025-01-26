using Komit.Base.Values.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
namespace Komit.Base.Dev.Server;
[Authorize]
[ApiController]
public class CqrsController : ControllerBase
{
    protected IKomitApp App { get; set; }
    public ISetSession Session { get; }
    public IDistributedCache SessionCache { get; }

    public CqrsController(IKomitApp app, ISetSession session, IDistributedCache sessionCache)
    {
        App = app;
        Session = session;
        SessionCache = sessionCache;
    }
    [HttpPost("{moduleName}/Api/Perform/{requestName}")]
    public async Task<IActionResult> Perform([FromRoute] string moduleName, [FromRoute] string requestName, [FromBody] object requestJson)
    {
        var sessionId = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "extension_SessionId")?.Value;
        var sessionString = SessionCache.GetString(sessionId);
        var session = JsonSerializer.Deserialize<SessionValue>(sessionString);
        Session.Set(session);
        var result = await App.Perform(moduleName, requestName, requestJson.ToString()!);
        return result.Success ? Ok(result) : StatusCode(500, result);
    }
    [HttpPost("{moduleName}/ApiJs/Command/{requestName}")]
    [HttpPost("{moduleName}/ApiJs/Query/{requestName}")]
    [HttpPost("{moduleName}/ApiJs/Perform/{requestName}")]
    public async Task<IActionResult> PerformJs([FromRoute] string moduleName, [FromRoute] string requestName, [FromBody] object requestJson)
    {
        var sessionId = HttpContext.User.Claims.SingleOrDefault(x => x.Type == "extension_SessionId")?.Value;
        var sessionString = SessionCache.GetString(sessionId);
        var session = JsonSerializer.Deserialize<SessionValue>(sessionString);
        Session.Set(session);
        var result = await App.Perform(moduleName, requestName, requestJson.ToString()!, true);
        return result.Success ? Ok(result) : StatusCode(500, result);
    }
}