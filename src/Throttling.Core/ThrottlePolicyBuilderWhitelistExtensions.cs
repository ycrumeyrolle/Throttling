using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class ThrottlePolicyBuilderWhitelistExtensions
    {
        /// <summary>
        /// Adds the IP range to the IP whitelist. 
        /// These addresses will pass through the policy.
        /// Valids patterns are : 
        /// <list type="square">
        ///     <item>Uni address: "127.0.0.1", ":;1",</item>
        ///     <item>Uni address list: "127.0.0.1", ":;1" separated by semi-colon,</item>
        ///     <item>CIDR block: "192.168.0.0/24", "fe80::/10",</item>
        ///     <item>Begin-end : "169.258.0.0-169.258.0.255",</item>
        ///     <item>Bit mask : "192.168.0.0/255.255.255.0".</item>
        /// </list>
        /// </summary>
        /// <param name="range">
        /// The IP addresses range. 
        /// </param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder IgnoreIPAddressRanges([NotNull] this ThrottlePolicyBuilder builder, [NotNull] params string[] ranges)
        {
            return builder.AddExclusions(new IPExclusion(ranges));
        }

        /// <summary>
        /// Adds the IP address to the IP whitelist. 
        /// These addresses will pass through the policy.
        /// Valids patterns are : 
        /// <list type="square">
        ///     <item>Uni address: "127.0.0.1", ":;1",</item>
        ///     <item>Uni address list: "127.0.0.1", ":;1" separated by semi-colon,</item>
        ///     <item>CIDR block: "192.168.0.0/24", "fe80::/10",</item>
        ///     <item>Begin-end : "169.258.0.0-169.258.0.255",</item>
        ///     <item>Bit mask : "192.168.0.0/255.255.255.0".</item>
        /// </list>
        /// </summary>
        /// <param name="address">
        /// The IP address. 
        /// </param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder IgnoreIPAddress([NotNull] this ThrottlePolicyBuilder builder, [NotNull] string address)
        {
            return builder.AddExclusions(new IPExclusion(new[] { address }));
        }
    }
}