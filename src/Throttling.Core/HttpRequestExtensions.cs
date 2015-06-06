using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Throttling.Core.Internal;

namespace Throttling
{
    public static class HttpRequestExtensions
    {
        public static void EnableCounting([NotNull] this HttpResponse response)
        {
            var body = response.Body;
            if (!body.CanRead)
            {
                response.Body = new CountingStream(body);
            }
        }
    }
}
