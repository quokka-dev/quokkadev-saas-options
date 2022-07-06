using Microsoft.Extensions.Options;
using Moq;
using QuokkaDev.Saas.Abstractions;
using System;
using Xunit;

namespace QuokkaDev.Saas.Options.Tests
{
    public class TenantOptionsCacheUnitTest
    {
        private readonly Mock<TenantOptionsCacheDictionary<TestOptions>> dictionaryMock;
        private readonly Mock<IOptionsMonitorCache<TestOptions>> optionsCacheMock;
        private readonly IOptionsMonitorCache<TestOptions> optionsCache;
        private readonly Mock<ITenantAccessor<Tenant<int>, int>> defaultTenantAccessorMock;
        private readonly Mock<ITenantAccessor<Tenant<int>, int>> nullTenantAccessorMock;

        public TenantOptionsCacheUnitTest()
        {
            optionsCacheMock = new();
            optionsCache = optionsCacheMock.Object;
            dictionaryMock = new();
            dictionaryMock.Setup(m => m.Get("my-tenant-identifier")).Returns(optionsCache);
            dictionaryMock.Setup(m => m.Get(It.Is<string>(s => !"my-tenant-identifier".Equals(s)))).Returns(new OptionsCache<TestOptions>());

            defaultTenantAccessorMock = new();
            defaultTenantAccessorMock.Setup(m => m.Tenant).Returns(new Tenant<int>(1, "my-tenant-identifier"));

            nullTenantAccessorMock = new();
            nullTenantAccessorMock.Setup(m => m.Tenant).Returns((Tenant<int>)null!);
        }

        [Fact(DisplayName = "TenantOptionCache should resolve right cache")]
        public void TenantOptionCache_Should_Resolve_Right_Cache()
        {
            // Arrange
            TenantOptionsCache<TestOptions, Tenant<int>, int> cache = new(defaultTenantAccessorMock.Object, dictionaryMock.Object);
            TenantOptionsCache<TestOptions, Tenant<int>, int> cacheForNullTenants = new(nullTenantAccessorMock.Object, dictionaryMock.Object);

            // Act
            cache.Clear(); // 1
            cache.GetOrAdd("Test", () => new TestOptions()); // 2
            cache.TryAdd("Test", new TestOptions()); // 3
            cache.TryRemove("Test"); // 4

            cacheForNullTenants.Clear(); // 5
            cacheForNullTenants.GetOrAdd("Test", () => new TestOptions()); // 6
            cacheForNullTenants.TryAdd("Test", new TestOptions()); // 7
            cacheForNullTenants.TryRemove("Test"); // 8

            // Assert
            dictionaryMock.Verify(m => m.Get("my-tenant-identifier"), Times.Exactly(4));
            dictionaryMock.Verify(m => m.Get(null), Times.Exactly(4));

            optionsCacheMock.Verify(m => m.Clear(), Times.Once);
            optionsCacheMock.Verify(m => m.GetOrAdd("Test", It.IsAny<Func<TestOptions>>()), Times.Once);
            optionsCacheMock.Verify(m => m.TryAdd("Test", It.IsAny<TestOptions>()), Times.Once);
            optionsCacheMock.Verify(m => m.TryRemove("Test"), Times.Once);
        }
    }
}
