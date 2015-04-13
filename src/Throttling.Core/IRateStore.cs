using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IRateStore
    {
        Task<RemainingRate> GetRemainingRateAsync([NotNull] string category, [NotNull] string endpoint, [NotNull] string key);

        Task SetRemainingRateAsync([NotNull] string category, [NotNull] string endpoint, [NotNull] string key, [NotNull]  RemainingRate remaining);
    }
}