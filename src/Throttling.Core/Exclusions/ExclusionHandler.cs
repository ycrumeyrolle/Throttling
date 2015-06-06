using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ExclusionHandler<TExclusion> : IExclusionHandler where TExclusion : IThrottlingExclusion
    {
        public virtual async Task HandleAsync([NotNull] ThrottlingContext throttlingContext)
        {
            foreach (var exclusion in throttlingContext.Exclusions.OfType<TExclusion>())
            {
                await HandleAsync(throttlingContext, exclusion);
            }
        }

        public abstract Task HandleAsync([NotNull] ThrottlingContext throttlingContext, [NotNull] TExclusion exclusion);
    }
}
