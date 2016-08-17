using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Throttling.Tests
{
    public class QueryStringApiKeyProviderTest
    {
        [Theory]
        [MemberData("Parameters")]
        public void GetKey_ReturnsFirstKey1(string[] values)
        {
            // Arrange
            var keyProvider = new QueryStringApiKeyProvider("apikey");
            var queryFeature = new QueryFeature(new QueryCollection(new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase) { { "apikey", values } }));
            var context = new DefaultHttpContext();
            context.Features[typeof(IQueryFeature)] = queryFeature;

            // Act
            var result = keyProvider.GetApiKey(context);

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
}
