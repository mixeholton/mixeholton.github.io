using Komit.Base.Module.Handlers.Context;
using Komit.Base.Specs.Fixtures;

namespace Komit.Base.Specs;
public class BaseAppStories<TDbContext, TModelContext, TReadContext> : BaseSpecs
        where TDbContext : KomitDbContext, TReadContext
        where TModelContext : ModelContext<TDbContext, TReadContext>
        where TReadContext : IReadContext
{
    public ServiceFixture Services { get; protected set; }
    public SessionFixture Session { get; protected set; }
    public AppFixture App { get; protected set; }
    public ModelFixture<TDbContext, TModelContext, TReadContext> Model { get; protected set; }
    public T ShouldSucceed<T>(RequestResult<T> request) => ShouldSucceedIf(request, true);
    public T ShouldFail<T>(RequestResult<T> request) => ShouldSucceedIf(request, false);
    public T ShouldSucceedIf<T>(RequestResult<T> request, bool expectation)
    {
        if (request.Success != expectation)
            throw new Exception($"{(expectation ? "Expected success" : "Expected failure")} for {request.Description}");
        return request.Result;
    }
    public async Task<IEnumerable<IdNamePair>> ShouldSucceed(Task<CommandResult> request) => ShouldSucceedIf(await request, true);
    public async Task<IEnumerable<IdNamePair>> ShouldFail(Task<CommandResult> request) => ShouldSucceedIf(await request, false);
    public async Task<IEnumerable<IdNamePair>> ShouldSucceedIf(Task<CommandResult> request, bool expectation) => ShouldSucceedIf(await request, expectation);
    public async Task<T> ShouldSucceed<T>(Task<RequestResult<T>> request) => ShouldSucceedIf(await request, true);
    public async Task<T> ShouldFail<T>(Task<RequestResult<T>> request) => ShouldSucceedIf(await request, false); 
    public async Task<T> ShouldSucceedIf<T>(Task<RequestResult<T>> request, bool expectation) => ShouldSucceedIf(await request, expectation);
}
