using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http.Features.Internal;
using Microsoft.AspNet.Http.Internal;
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
            var queryFeature = new QueryFeature(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase) { { "apikey", values } });
            var context = new DefaultHttpContext();
            context.SetFeature<IQueryFeature>(queryFeature);

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
