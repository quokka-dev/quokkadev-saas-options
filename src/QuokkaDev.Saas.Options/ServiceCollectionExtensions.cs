using Autofac;
using Microsoft.Extensions.Options;
using QuokkaDev.Saas.Abstractions;

namespace QuokkaDev.Saas.Options
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register tenant specific options
        /// </summary>
        /// <typeparam name="TOptions">Type of options we are apply configuration to</typeparam>
        /// <param name="tenantOptionsConfiguration">Action to configure options for a tenant</param>
        /// <returns></returns>
        public static ContainerBuilder RegisterTenantOptions<TOptions, T, TKey>(this ContainerBuilder builder, Action<TOptions, T> tenantConfig) where TOptions : class, new() where T : Tenant<TKey>
        {
            builder.RegisterType<TenantOptionsCacheDictionary<TOptions>>()
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<TenantOptionsCache<TOptions, T, TKey>>()
                .As<IOptionsMonitorCache<TOptions>>()
                .SingleInstance();

            builder.RegisterType<TenantOptionsFactory<TOptions, T, TKey>>()
                .As<IOptionsFactory<TOptions>>()
                .WithParameter(new TypedParameter(typeof(Action<TOptions, T>), tenantConfig))
                .SingleInstance();

            builder.RegisterType<IOptions<TOptions>>()
                .As<IOptionsSnapshot<TOptions>>()
                .SingleInstance();

            builder.RegisterType<IOptions<TOptions>>()
                .As<IOptions<TOptions>>()
                .SingleInstance();

            return builder;
        }
    }
}
