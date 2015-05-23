using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features.Internal;
using Microsoft.AspNet.Http.Internal;
using Xunit;

namespace Throttling.Tests
{
    public class QueryStringClientKeyProviderTest
    {
        [Theory]
        [MemberData("Parameters")]
        public void GetKey_ReturnsFirstKey1(string[] values)
        {
            // Arrange
            var keyProvider = new QueryStringClientKeyProvider("apikey");
            var queryFeature = new QueryFeature(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase) { { "apikey", values } });
            var context = new DefaultHttpContext();
            context.SetFeature<IQueryFeature>(queryFeature);

            // Act
            var result = keyProvider.GetKey(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(values[0], result);
        }

        public static IEnumerable<object[]> Parameters
        {
            get
            {
                return ApiKeys.Values;
            }
        }
    }

    public class RouteClientKeyProviderTest
    {
        [Theory]
        [InlineData("{apikey}/{*remaining}", "/apikeyvalue", "apikeyvalue")]
        [InlineData("{apikey}/{*remaining}", "/apikeyvalue/otherthings", "apikeyvalue")]
        [InlineData("XXX/{apikey}/{*remaining}", "/XXX/apikeyvalue/otherthings", "apikeyvalue")]
        [InlineData("{apikey}/{*remaining}", "/apikeyvalue/apikeyvalue", "apikeyvalue")]
        public void GetKey_ReturnsKey(string routeTemplate, string path, string expectedKey)
        {
            // Arrange
            var keyProvider = new RouteClientKeyProvider(routeTemplate, "apikey");
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString(path);

            // Act 
            var result = keyProvider.GetKey(context);

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
            var keyProvider = new RouteClientKeyProvider(routeTemplate, "apikey");
            var context = new DefaultHttpContext();
            context.Request.Path = new PathString(path);

            // Act
            var result = keyProvider.GetKey(context);

            // Assert
            Assert.Null(result);
        }
    }

    public class FormClientKeyProviderTest
    {
        [Theory]
        [MemberData("Parameters")]
        public void GetKey_ReturnsFirstKey1(string[] values)
        {
            // Arrange
            var keyProvider = new FormClientKeyProvider("apikey");
            IFormFeature formFeature = new FormFeature(new FormCollection(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase) { { "apikey", values } }));
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.SetFeature(formFeature);

            // Act
            var result = keyProvider.GetKey(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(values[0], result);
        }

        public static IEnumerable<object[]> Parameters
        {
            get
            {
                return ApiKeys.Values;
            }
        }
    }

    public class HeaderClientKeyProviderTest
    {
        [Theory]
        [MemberData("Parameters")]
        public void GetKey_ReturnsFirstKey(string[] values)
        {
            // Arrange
            var keyProvider = new HeaderClientKeyProvider("apikey");
            var context = new DefaultHttpContext();
            context.Request.Headers.SetValues("apikey", values);

            // Act
            var result = keyProvider.GetKey(context);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(values[0], result);
        }

        public static IEnumerable<object[]> Parameters
        {
            get
            {
                return ApiKeys.Values;
            }
        }
    }

    public static class ApiKeys
    {
        public static IEnumerable<object[]> Values
        {
            get
            {
                return new[]
                {
                    new[] { new[] { "apikeyvalue" } },
                    new[] { new[] { "apikeyvalue1", "apikeyvalue2", "apikeyvalue2" } }
                };
            }
        }
    }
}
