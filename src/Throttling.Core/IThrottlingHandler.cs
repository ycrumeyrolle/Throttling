using System.Threading.Tasks;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottlingHandler
    {
        Task HandleAsync([NotNull] ThrottlingContext context);

        Task PostHandleAsync([NotNull] ThrottlingContext throttlingContext);
    }
}
