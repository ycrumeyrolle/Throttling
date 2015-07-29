using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ExclusionHandler<TExclusion> : IExclusionHandler where TExclusion : IThrottleExclusion
    {
        public virtual async Task HandleExclusionAsync([NotNull] ThrottleContext throttleContext)
        {
            foreach (var exclusion in throttleContext.Exclusions.OfType<TExclusion>())
            {
                await HandleAsync(throttleContext, exclusion);
            }
        }

        public abstract Task HandleAsync([NotNull] ThrottleContext throttleContext, [NotNull] TExclusion exclusion);
    }
}
