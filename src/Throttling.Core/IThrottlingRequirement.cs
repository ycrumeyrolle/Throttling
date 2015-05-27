using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public interface IThrottlingRequirement
    {        
    }

    //public class DefaultThrottlingService : IThrottlingService2
    //{
    //    private readonly IList<IThrottlingHandler> _handlers;
    //    private readonly ThrottlingOptions _options;

    //    public DefaultThrottlingService(IOptions<ThrottlingOptions> options, IEnumerable<IThrottlingHandler> handlers)
    //    {
    //        _handlers = handlers.ToArray();
    //        _options = options.Options;
    //    }

    //    public async Task<bool> EnsureThottlintAsync(object resource, [NotNull] IEnumerable<IThrottlingRequirement> requirements)
    //    {
    //        var throttlingContext = new ThrottlingContext(requirements, resource);
    //        foreach (var handler in _handlers)
    //        {
    //            await handler.HandleAsync(throttlingContext);
    //        }

    //        return throttlingContext.HasSucceeded;
    //    }

    //    public Task<bool> EnsureThottlintAsync(object resource, string policyName)
    //    {
    //        var policy = _options.GetPolicy(policyName);
    //        return (policy == null)
    //            ? Task.FromResult(false)
    //            : this.EnsureThottlintAsync(resource, policy);
    //    }
    //}
}