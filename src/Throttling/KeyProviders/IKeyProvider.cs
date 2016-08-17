using Microsoft.AspNetCore.Http;

namespace Throttling
{
    /// <summary>
    /// Provides a method to retrieve the key (TODO : be more precise). 
    /// </summary>
    public interface IKeyProvider
    {
        string GetKey(HttpContext httpContext);
    }
}