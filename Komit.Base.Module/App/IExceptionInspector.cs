namespace Komit.Base.Module.App;
public interface IExceptionInspector
{
    void Thrown(Exception exception, bool isHandled);
    void UsedForErrorMessage(Exception exception, bool isHandled);
}
