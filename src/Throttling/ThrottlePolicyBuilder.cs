using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottlePolicyBuilder : IThrottlePolicyBuilder
    {
        private readonly string _policyName;

        public ThrottlePolicyBuilder()
            : this(string.Empty)
        {            
        }

        public ThrottlePolicyBuilder(string policyName)
        {
            _policyName = policyName;
        }

        private ThrottlePolicyBuilder(string policyName, IList<IThrottleRequirement> requirements, IList<IThrottleExclusion> exclusions)
        {
            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            if (exclusions == null)
            {
                throw new ArgumentNullException(nameof(exclusions));
            }

            _policyName = policyName;
            Requirements = requirements;
            Exclusions = exclusions;
        }

        public IList<IThrottleRequirement> Requirements { get; } = new List<IThrottleRequirement>();

        public IList<IThrottleExclusion> Exclusions { get; } = new List<IThrottleExclusion>();

        public string Name
        {
            get
            {
                return _policyName;
            }
        }


        /// <summary>
        /// Adds a list of requirements.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ThrottlePolicyBuilder AddRequirements(params IThrottleRequirement[] requirements)
        {
            foreach (var requirement in requirements)
            {
                Requirements.Add(requirement);
            }

            return this;
        }

        /// <summary>
        /// Adds a list of exclusions.
        /// </summary>
        /// <returns>The current policy builder.</returns>
        public ThrottlePolicyBuilder AddExclusions(params IThrottleExclusion[] exclusions)
        {
            foreach (var exclusion in exclusions)
            {
                Exclusions.Add(exclusion);
            }

            return this;
        }

        public ThrottlePolicy Build(ThrottleOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            
            return new ThrottlePolicy(Requirements, Exclusions, _policyName);
        }

        public static ThrottlePolicyBuilder FromBuilder(ThrottlePolicyBuilder builder, string policyName)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return new ThrottlePolicyBuilder(policyName, builder.Requirements, builder.Exclusions);
        }
    }
}