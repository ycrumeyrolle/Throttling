using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ThrottlingHandler<TRequirement> : IThrottlingHandler where TRequirement : IThrottlingRequirement
    {
        public virtual async Task HandleAsync([NotNull] ThrottlingContext context)
        {
            foreach (var requirement in context.Requirements.OfType<TRequirement>())
            {
                await HandleAsync(context, requirement);
            }
        }

        public abstract Task HandleAsync([NotNull] ThrottlingContext context, [NotNull] TRequirement requirement);
    }
}