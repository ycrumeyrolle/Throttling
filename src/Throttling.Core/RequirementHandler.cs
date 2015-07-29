using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class RequirementHandler<TRequirement> : IRequirementHandler where TRequirement : IThrottleRequirement
    {
        public virtual async Task HandleRequirementAsync([NotNull] ThrottleContext throttleContext)
        {
            foreach (var requirement in throttleContext.Requirements.OfType<TRequirement>())
            {
                await HandleAsync(throttleContext, requirement);
            }
        }

        public virtual async Task PostHandleRequirementAsync([NotNull] ThrottleContext throttleContext)
        {
            foreach (var requirement in throttleContext.Requirements.OfType<TRequirement>())
            {
                await PostHandleAsync(throttleContext, requirement);
            }
        }

        public abstract Task HandleAsync([NotNull] ThrottleContext throttleContext, [NotNull] TRequirement requirement);

        public virtual Task PostHandleAsync([NotNull] ThrottleContext throttleContext, [NotNull] TRequirement requirement)
        {
            return Constants.CompletedTask;
        }
    }
}