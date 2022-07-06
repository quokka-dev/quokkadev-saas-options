using FluentAssertions;
using Xunit;

namespace QuokkaDev.Saas.Options.Tests
{
    public class TenantOptionsCacheDictionaryUnitTest
    {
        public TenantOptionsCacheDictionaryUnitTest()
        {
        }

        [Fact(DisplayName = "TenantOptionsCacheDictionary should works as expected")]
        public void TenantOptionsCacheDictionary_Should_Works_As_Expected()
        {
            // Arrange
            TenantOptionsCacheDictionary<TestOptions> dictionary = new();

            // Act
            var cache1 = dictionary.Get("my-tenant-identifier");
            var cache2 = dictionary.Get("test");
            var cache3 = dictionary.Get("my-tenant-identifier");

            // Assert
            cache1.Should().NotBeNull();
            cache2.Should().NotBeNull();
            cache3.Should().NotBeNull();

            cache1.Should().NotBeSameAs(cache2);
            cache2.Should().NotBeSameAs(cache3);
            cache3.Should().BeSameAs(cache1);
        }
    }
}
