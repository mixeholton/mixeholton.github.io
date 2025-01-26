using Komit.Base.Module.App;

namespace Komit.Sandbox.Web.Services;

public class ExceptionInspector : IExceptionInspector
{
    public void Thrown(Exception exception, bool isHandled)
    {
    }

    public void UsedForErrorMessage(Exception exception, bool isHandled)
    {
    }
}
