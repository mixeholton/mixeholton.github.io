using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;

namespace Komit.Base.Specs.Stubs;
public class _SessionCacheStub : IDistributedCache
{
    protected static readonly ConcurrentDictionary<string, byte[]> Cache = new();
    public byte[]? Get(string key) => Cache.TryGetValue(key, out var value) ? value : null;
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options) => Cache.TryAdd(key, value);
    public void Remove(string key) => Cache.TryRemove(key, out _);
    public void Refresh(string key) { }
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default) => Get(key);
    public async Task RefreshAsync(string key, CancellationToken token = default) => Refresh(key);
    public async Task RemoveAsync(string key, CancellationToken token = default) => Remove(key);
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default) => Set(key, value, options);
}
