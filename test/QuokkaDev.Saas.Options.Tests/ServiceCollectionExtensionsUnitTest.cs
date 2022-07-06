using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using QuokkaDev.Saas.Abstractions;
using QuokkaDev.Saas.DependencyInjection;
using QuokkaDev.Saas.ServiceProvider;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.Saas.Options.Tests;

public class ServiceCollectionExtensionsUnitTest
{
    [Fact(DisplayName = "RegisterTenantOptions should register options")]
    public void RegisterTenantOptions_Should_Register_Options()
    {
        // Arrange        
        FakeTenantAccessService service = new();
        MultiTenantContainer<Tenant<int>, int> container;
        IContainer applicationContainer;
        (container, applicationContainer) = GetMultiTenantContainer(service, (_, builder) =>
        {
            builder.RegisterTenantOptions<TestOptions, Tenant<int>, int>((config, tenant) =>
            {
                config.IsOptionConfigured = true;
                config.Tenant = tenant.Identifier;
            });
        });

        // Act
        var scope1 = container.GetCurrentTenantScope();
        var options1 = scope1.Resolve<IOptions<TestOptions>>().Value; //Make a snapshot

        service.SetTenant("other-tenant");

        var scope2 = container.GetCurrentTenantScope();
        var options2 = scope2.Resolve<IOptions<TestOptions>>().Value; //Make a snapshot

        // Assert
        options1.Should().NotBeNull();
        options1.Tenant.Should().Be("my-tenant-identifier");
        options1.IsOptionConfigured.Should().BeTrue();

        options2.Should().NotBeNull();
        options2.Tenant.Should().Be("other-tenant");
        options2.IsOptionConfigured.Should().BeTrue();
    }

    private static (MultiTenantContainer<Tenant<int>, int> Container, IContainer RootContainer) GetMultiTenantContainer(FakeTenantAccessService service, Action<Tenant<int>, ContainerBuilder>? config = null)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddMultiTenancy<Tenant<int>, int>()
            .WithService<FakeTenantAccessService>(service);

        FakeTenantAccessor accessor = new(service);
        services.AddSingleton<ITenantAccessor<Tenant<int>, int>>(accessor);

        ContainerBuilder builder = new();
        builder.Populate(services);
        var applicationContainer = builder.Build();

#pragma warning disable RCS1163 // Unused parameter.
        MultiTenantContainer<Tenant<int>, int> container = new(applicationContainer, config ?? ((tenant, builder) => { }));
#pragma warning restore RCS1163 // Unused parameter.
        return (container, applicationContainer);
    }
}

public class TestOptions
{
    public string? Tenant { get; set; }
    public bool IsOptionConfigured { get; set; }
}

public class FakeTenantAccessService : ITenantAccessService<Tenant<int>, int>
{
    private string currentTenantIdentifier = "my-tenant-identifier";

    public void SetTenant(string newTenantIdentifier)
    {
        currentTenantIdentifier = newTenantIdentifier;
    }

    public Tenant<int> GetTenant()
    {
        return new Tenant<int>(1, currentTenantIdentifier);
    }

    public Task<Tenant<int>> GetTenantAsync()
    {
        return Task.FromResult(GetTenant());
    }
}

public class FakeTenantAccessor : ITenantAccessor<Tenant<int>, int>
{
    private readonly FakeTenantAccessService service;

    public FakeTenantAccessor(FakeTenantAccessService service)
    {
        this.service = service;
    }

    public Tenant<int>? Tenant => service.GetTenant();
}