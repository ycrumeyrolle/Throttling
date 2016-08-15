using System;

namespace Throttling.Mvc
{
    /// <summary>
    /// Identify a an action to disable throttling.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DisableThrottlingAttribute : Attribute, IDisableThrottlingAttribute
    {
    }
}
