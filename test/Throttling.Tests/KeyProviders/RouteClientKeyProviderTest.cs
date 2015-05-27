using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Xunit;

namespace Throttling.Tests
{
    public class RouteApiKeyProviderTest
    {
        [Theory]
        [InlineData("{apikeyA}", "apikeyB")]
        [InlineData("{*apikeyA}", "apikeyB")]
        [InlineData("{?apikeyA}", "apikeyB")]
        [InlineData("{apikeyA}/{*remaining}", "apikeyB")]
        [InlineData("XXX/{apikeyA}/{*remaining}", "apikeyB")]
        public void Ctor_IncorrectApiKeyName_ThrowsException(string routeTemplate, string apiKeyName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new RouteApiKeyProvider(routeTemplate, apiKeyName));
        }

        [Theory]
        [InlineData("{apikey}/{*remaining}", "/apikeyvalue", "apikeyvalue")]
        [InlineData("{apikey}/{*remaining}", "/apikeyvalue/otherthings", "apikeyvalue")]
        [InlineData("XXX/{apikey}/{*remaining}", "/XXX/apikeyvalue/otherthings", "apikeyvalue")]
        [InlineData("{apikey}/{*remaining}", "/apikeyvalue/apikeyvalue", "apikeyvalue")]
        public void GetKey_ReturnsKey(string routeTemplate, string path, string expectedKey)
        {
            // Arrange
            var keyProvider = new RouteApiKeyProvider(routeTemplate, "apikey");
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString(path);

            // Act 
            var result = keyProvider.GetApiKey(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedKey, result);
        }

        [Theory]
        [InlineData("{apikey}/{*remaining}", "/")]
        [InlineData("XXX/{apikey}/{*remaining}", "/ZZZ/apikeyvalue/otherthings")]
        public void GetKey_InvalidRoute_ReturnsNull(string routeTemplate, string path)
        {
            // Arrange
            var keyProvider = new RouteApiKeyProvider(routeTemplate, "apikey");
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString(path);

            // Act
            var result = keyProvider.GetApiKey(context);

            // Assert
            Assert.Null(result);
        }
    }
}
