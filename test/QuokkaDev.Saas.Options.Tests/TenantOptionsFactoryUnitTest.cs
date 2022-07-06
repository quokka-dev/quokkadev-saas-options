using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using QuokkaDev.Saas.Abstractions;
using System.Collections.Generic;
using Xunit;

namespace QuokkaDev.Saas.Options.Tests
{
    public class TenantOptionsFactoryUnitTest
    {
        public TenantOptionsFactoryUnitTest()
        {
        }

        [Fact(DisplayName = "Factory should create configured options")]
        public void Factory_Should_Create_Configured_Options()
        {
            // Arrange
            var setupsMock = new Mock<IConfigureOptions<TestOptions>>();
            var namedSetupsMock = new Mock<IConfigureNamedOptions<TestOptions>>();
            var postConfigureSetupMock = new Mock<IPostConfigureOptions<TestOptions>>();

            var setups = new List<IConfigureOptions<TestOptions>>() { setupsMock.Object, namedSetupsMock.Object };
            var postConfigureSetups = new List<IPostConfigureOptions<TestOptions>>() { postConfigureSetupMock.Object };

            var tenantAccessorMock = new Mock<ITenantAccessor<Tenant<int>, int>>();
            tenantAccessorMock.Setup(m => m.Tenant).Returns(new Tenant<int>(1, "my-tenant-identifier"));

            var factory = new TenantOptionsFactory<TestOptions, Tenant<int>, int>(
                setups,
                postConfigureSetups,
                (opts, _) => opts.Tenant = "configured-tenant",
                tenantAccessorMock.Object
                );

            // Act
            var options = factory.Create("Name");

            // Assert
            options.Should().NotBeNull();
            options.Tenant.Should().Be("configured-tenant");

            setupsMock.Verify(m => m.Configure(It.IsAny<TestOptions>()), Times.Once);
            namedSetupsMock.Verify(m => m.Configure("Name", It.IsAny<TestOptions>()), Times.Once);
            postConfigureSetupMock.Verify(m => m.PostConfigure("Name", It.IsAny<TestOptions>()), Times.Once);
        }
    }
}
