using Microsoft.AspNet.Mvc;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which can be used to enable/disable throttling support for a resource.
    /// </summary>
    public interface IThrottlingFilter : IAsyncAuthorizationFilter, IOrderedFilter, IActionFilter
    {
        ThrottlingRoute Route { get; set; }
    }
}
