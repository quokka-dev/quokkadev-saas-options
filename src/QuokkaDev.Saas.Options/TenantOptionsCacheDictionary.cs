using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace QuokkaDev.Saas.Options
{
    /// <summary>
    /// Dictionary of tenant specific options caches
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class TenantOptionsCacheDictionary<TOptions> where TOptions : class
    {
        /// <summary>
        /// Caches stored in memory
        /// </summary>
        private readonly ConcurrentDictionary<string, IOptionsMonitorCache<TOptions>> _tenantSpecificOptionCaches = new();

        /// <summary>
        /// Get options for specific tenant (create if not exists)
        /// </summary>
        /// <param name="tenantIdentifier"></param>
        /// <returns></returns>
        public virtual IOptionsMonitorCache<TOptions> Get(string? tenantIdentifier)
        {
            return _tenantSpecificOptionCaches.GetOrAdd(tenantIdentifier ?? "", new OptionsCache<TOptions>());
        }
    }
}
