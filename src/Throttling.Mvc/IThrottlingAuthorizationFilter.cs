using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Internal;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which can be used to enable/disable throttling support for a resource.
    /// </summary>
    public interface IThrottlingAuthorizationFilter : IAsyncAuthorizationFilter, IOrderedFilter
    {
        string PolicyName { get; set; }
    }
}
