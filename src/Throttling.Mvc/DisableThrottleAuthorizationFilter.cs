using Microsoft.AspNet.Mvc.Filters;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which can be used to enable throttling support for a resource.
    /// </summary>
    public class DisableThrottleAuthorizationFilter : IFilterMetadata
    {
    }
}