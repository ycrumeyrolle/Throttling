using Microsoft.AspNet.Http;

namespace Throttling
{
    /// <summary>
    /// Provides a method to retrieve the API key. 
    /// </summary>
    public interface IApiKeyProvider
    {
        string GetApiKey(HttpContext httpContext);
    }
}