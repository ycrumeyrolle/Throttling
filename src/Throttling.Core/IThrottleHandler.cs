using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottleHandler
    {
        Task HandleAsync([NotNull] ThrottleContext throttleContext);

        Task PostHandleAsync([NotNull] ThrottleContext throttleContext);
    }
}
