using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ThrottlingHandler<TRequirement> : IThrottlingHandler where TRequirement : IThrottlingRequirement
    {
        public virtual async Task HandleAsync([NotNull] ThrottlingContext throttlingContext)
        {
            foreach (var requirement in throttlingContext.Requirements.OfType<TRequirement>())
            {
                await HandleAsync(throttlingContext, requirement);
            }
        }

        public virtual async Task PostHandleAsync([NotNull] ThrottlingContext throttlingContext)
        {
            foreach (var requirement in throttlingContext.Requirements.OfType<TRequirement>())
            {
                await PostHandleAsync(throttlingContext, requirement);
            }
        }

        public abstract Task HandleAsync([NotNull] ThrottlingContext throttlingContext, [NotNull] TRequirement requirement);

        public virtual Task PostHandleAsync([NotNull] ThrottlingContext throttlingContext, [NotNull] TRequirement requirement)
        {
            return Constants.CompletedTask;
        }
    }
}