using System;
using System.Linq;
using System.Threading.Tasks;

namespace Throttling
{
    public abstract class RequirementHandler<TRequirement> : IRequirementHandler where TRequirement : IThrottleRequirement
    {
        public virtual async Task HandleRequirementAsync(ThrottleContext throttleContext)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            foreach (var requirement in throttleContext.Requirements.OfType<TRequirement>())
            {
                await HandleRequirementAsync(throttleContext, requirement);
            }
        }

        public virtual async Task PostHandleRequirementAsync(ThrottleContext throttleContext)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            foreach (var requirement in throttleContext.Requirements.OfType<TRequirement>())
            {
                await PostHandleRequirementAsync(throttleContext, requirement);
            }
        }

        public abstract Task HandleRequirementAsync(ThrottleContext throttleContext, TRequirement requirement);

        public virtual Task PostHandleRequirementAsync(ThrottleContext throttleContext, TRequirement requirement)
        {
            return Constants.CompletedTask;
        }
    }
}