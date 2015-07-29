using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IRequirementHandler
    {
        Task HandleRequirementAsync([NotNull] ThrottleContext throttleContext);

        Task PostHandleRequirementAsync([NotNull] ThrottleContext throttleContext);
    }
}
