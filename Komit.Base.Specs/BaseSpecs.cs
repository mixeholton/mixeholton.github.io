namespace Komit.Base.Specs;
public abstract class BaseSpecs
{
    // Use these for XUnit [MemberData] [Theory] spec convienience
    public static IEnumerable<object[]> AsScenarios<T>(IEnumerable<T> list) => list.Select(x => new object[] { x }).Cast<object[]>().ToList();
    public static object[] Scenario(params object[] input) => input;
    public static object[] Scenario<T>(params T[] input) => input.Cast<object>().ToArray();
    public void ShouldSucceed(Action action) => ShouldSucceedIf(action, true);
    public void ShouldFail(Action action) => ShouldSucceedIf(action, false);
    public void ShouldSucceedIf(Action action, bool expectation)
    {
        var success = false;
        try
        {
            action();
            success = true;
        }
        catch
        {
            if (expectation)
                throw;
        }
        if (!expectation && success)
            throw new Exception("Expected failure");
    }
}