namespace Throttling.Mvc
{
    /// <summary>
    /// An interface which can be used to identify a type which provides metdata to enable throttling for a resource.
    /// </summary>
    public interface IEnableThrottlingAttribute
    {
        /// <summary>
        /// The name of the policy which needs to be applied.
        /// </summary>
        string PolicyName { get; set; }
    }
}
