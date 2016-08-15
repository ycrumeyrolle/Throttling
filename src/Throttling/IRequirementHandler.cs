using System.Threading.Tasks;

namespace Throttling
{
    public interface IRequirementHandler
    {
        Task HandleRequirementAsync(ThrottleContext throttleContext);

        Task PostHandleRequirementAsync(ThrottleContext throttleContext);
    }
}
