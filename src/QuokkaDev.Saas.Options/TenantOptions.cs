using Microsoft.Extensions.Options;

namespace QuokkaDev.Saas.Options
{
    /// <summary>
    /// Make IOptions tenant aware
    /// </summary>
    public class IOptions<TOptions> : IOptionsSnapshot<TOptions>
        where TOptions : class, new()
    {
        private readonly IOptionsFactory<TOptions> _factory;
        private readonly IOptionsMonitorCache<TOptions> _cache;

        public IOptions(IOptionsFactory<TOptions> factory, IOptionsMonitorCache<TOptions> cache)
        {
            _factory = factory;
            _cache = cache;
        }

        public TOptions Value => Get(Microsoft.Extensions.Options.Options.DefaultName);

        public TOptions Get(string name)
        {
            return _cache.GetOrAdd(name, () => _factory.Create(name));
        }
    }
}
