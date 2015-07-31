﻿using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which can be used to enable/disable throttling support for a resource.
    /// </summary>
    public interface IThrottleFilter : IAsyncAuthorizationFilter, IOrderedFilter, IAsyncResultFilter
    {
        ThrottleRouteCollection Routes { get; set; }
    }
}
