using Microsoft.Extensions.Options;
using QuokkaDev.Saas.Abstractions;

namespace QuokkaDev.Saas.Options
{
    /// <summary>
    /// Tenant aware options cache
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <typeparam name="TTenant"></typeparam>
    public class TenantOptionsCache<TOptions, TTenant, TKey> : IOptionsMonitorCache<TOptions>
        where TOptions : class
        where TTenant : Tenant<TKey>
    {
        private readonly ITenantAccessor<TTenant, TKey> _tenantAccessor;
        private readonly TenantOptionsCacheDictionary<TOptions> _tenantSpecificOptionsCache;

        public TenantOptionsCache(ITenantAccessor<TTenant, TKey> tenantAccessor, TenantOptionsCacheDictionary<TOptions> tenantSpecificOptionsCache)
        {
            _tenantAccessor = tenantAccessor;
            _tenantSpecificOptionsCache = tenantSpecificOptionsCache;
        }

        public void Clear()
        {
            _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant?.Identifier).Clear();
        }

        public TOptions GetOrAdd(string name, Func<TOptions> createOptions)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant?.Identifier)
                .GetOrAdd(name, createOptions);
        }

        public bool TryAdd(string name, TOptions options)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant?.Identifier)
                .TryAdd(name, options);
        }

        public bool TryRemove(string name)
        {
            return _tenantSpecificOptionsCache.Get(_tenantAccessor.Tenant?.Identifier)
                .TryRemove(name);
        }
    }
}
