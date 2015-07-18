using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Throttling.Internal;

namespace Throttling
{
    public static class HttpResponseExtensions
    {
        public static void TrackContentLength([NotNull] this HttpResponse response, [NotNull]ThrottleContext throttleContext)
        {
            var body = response.Body as ContentLengthTrackingStream;
            if (body == null)
            {
                var tracker = new ContentLengthTracker();
                response.Body = new ContentLengthTrackingStream(response.Body, tracker);
                throttleContext.ContentLengthTracker = tracker;
            }     
        }
    }
}
