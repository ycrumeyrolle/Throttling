using System;

namespace Throttling.Mvc
{
    /// <summary>
    /// Identify a controller or an action to enable throttling.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EnableThrottlingAttribute : Attribute, IEnableThrottlingAttribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EnableThrottlingAttribute"/>.
        /// </summary>
        /// <param name="policyName">The name of the policy to be applied.</param>
        public EnableThrottlingAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        /// <inheritdoc />
        public string PolicyName { get; set; }
    }
}
