using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace QuokkaDev.Saas.Options.Tests
{
    public class IOptionsUnitTest
    {
        private readonly TestOptions cachedTenantOptions = new() { IsOptionConfigured = true, Tenant = "my-tenant-identifier" };
        private readonly TestOptions newTenantOptions = new() { IsOptionConfigured = true, Tenant = "other-tenant" };
        private readonly Mock<IOptionsFactory<TestOptions>> factoryMock;
        private readonly Mock<IOptionsMonitorCache<TestOptions>> cacheMock;
        public IOptionsUnitTest()
        {
            factoryMock = new();
            factoryMock.Setup(m => m.Create(It.IsAny<string>())).Returns(newTenantOptions);

            cacheMock = new();
            cacheMock.Setup(m => m.GetOrAdd("my-tenant-identifier", It.IsAny<Func<TestOptions>>())).Returns(cachedTenantOptions);
            cacheMock.Setup(m => m.GetOrAdd("other-tenant", It.IsAny<Func<TestOptions>>())).Returns(() => factoryMock.Object.Create("other-tenant"));
        }

        [Fact(DisplayName = "IOptions should hit cache value")]
        public void IOptions_Should_Hit_Cache_Value()
        {
            // Arrange  
            IOptions<TestOptions> options = new(factoryMock.Object, cacheMock.Object);

            // Act
            var resultOptions = options.Get("my-tenant-identifier");

            // Assert
            resultOptions.Should().NotBeNull();
            resultOptions.Tenant.Should().Be("my-tenant-identifier");
            factoryMock.Verify(m => m.Create(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Factory should be invoked")]
        public void Factory_Should_Be_Invoked()
        {
            // Arrange  
            IOptions<TestOptions> options = new(factoryMock.Object, cacheMock.Object);

            // Act
            var resultOptions = options.Get("other-tenant");

            // Assert
            resultOptions.Should().NotBeNull();
            resultOptions.Tenant.Should().Be("other-tenant");
            factoryMock.Verify(m => m.Create(It.IsAny<string>()), Times.Once);
        }
    }
}
