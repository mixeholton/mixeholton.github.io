using Microsoft.AspNetCore.Mvc;

namespace Komit.Sandbox.Web.Controllers;

[ApiController]
[Route("Api/[controller]/[action]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJPeg()
    {
        var image = await new HttpClient().GetStreamAsync("https://komit.nu/wp-content/uploads/2023/05/billede-6-uai-258x391.jpg");
        return File(image, "image/jpeg");
    }
    [HttpGet("path")]
    public async Task<IActionResult> GetMessageFrom([FromRoute] string path)
    {
        var message = await new HttpClient().GetStringAsync(path);
        return Ok(message);
    }
    [HttpGet]
    public async Task<IActionResult> AMessage()
    {
        return Ok("Dont panic!");
    }
}
