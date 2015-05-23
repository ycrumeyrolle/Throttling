using Microsoft.AspNet.Http;

namespace Throttling
{
    public interface IClientKeyProvider
    {
        string GetKey(HttpContext context);
    }
}