using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottlingPolicy
    {
        string Category { get; set; }

        Task<IEnumerable<ThrottlingResult>> EvaluateAsync([NotNull] HttpContext context);

        void Configure(ThrottlingOptions options);
    }
}