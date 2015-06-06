using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    /// <summary>
    /// Provides a method to retrieve the key (TODO : be more precise). 
    /// </summary>
    public interface IKeyProvider
    {
        string GetKey([NotNull]HttpContext context);
    }
}