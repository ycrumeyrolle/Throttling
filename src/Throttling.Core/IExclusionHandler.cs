using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IExclusionHandler
    {
        Task HandleExclusionAsync([NotNull] ThrottleContext throttleContext);
    }
}