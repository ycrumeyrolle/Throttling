using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http.Features.Internal;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Framework.Primitives;
using Xunit;

namespace Throttling.Tests
{
    public class FormApiKeyProviderTest
    {
        [Theory]
        [MemberData("Parameters")]
        public void GetKey_ReturnsFirstKey1(string[] values)
        {
            // Arrange
            var keyProvider = new FormApiKeyProvider("apikey");
            IFormFeature formFeature = new FormFeature(new FormCollection(new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase) { { "apikey", values } }));
            var context = new DefaultHttpContext();
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Features[typeof(IFormFeature)] = formFeature;

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
