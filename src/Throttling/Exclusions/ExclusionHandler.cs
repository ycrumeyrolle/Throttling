using System;
using System.Linq;
using System.Threading.Tasks;

namespace Throttling
{
    public abstract class ExclusionHandler<TExclusion> : IExclusionHandler where TExclusion : IThrottleExclusion
    {
        public virtual async Task HandleExclusionAsync(ThrottleContext throttleContext)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            foreach (var exclusion in throttleContext.Exclusions.OfType<TExclusion>())
            {
                await HandleAsync(throttleContext, exclusion);
            }
        }

        public abstract Task HandleAsync(ThrottleContext throttleContext, TExclusion exclusion);
    }
}
