using System.Threading.Tasks;

namespace Throttling
{
    public interface IExclusionHandler
    {
        Task HandleExclusionAsync(ThrottleContext throttleContext);
    }
}