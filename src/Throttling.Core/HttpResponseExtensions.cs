using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Throttling.Core.Internal;

namespace Throttling
{
    public static class HttpResponseExtensions
    {
        public static ContentLengthTracker TrackContentLength([NotNull] this HttpResponse response)
        {
            ContentLengthTracker tracker;
            var body = response.Body as ContentLengthTrackingStream;
            if (body == null)
            {
                tracker = new ContentLengthTracker();
                response.Body = new ContentLengthTrackingStream(response.Body, tracker);
            }
            else
            {
                tracker = body.Tracker;
            }

            return tracker;
        }
    }
}
