using System;
using Microsoft.AspNetCore.Http;
using Throttling.Internal;

namespace Throttling
{
    public static class HttpResponseExtensions
    {
        public static void TrackContentLength(this HttpResponse response, ThrottleContext throttleContext)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

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
