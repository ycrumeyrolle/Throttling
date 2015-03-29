// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;
using Throttling;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions for enabling Throttling support.
    /// </summary>
    public static class ThrottlingServiceCollectionExtensions
    {
        /// <summary>
        /// Can be used to configure services in the <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection which needs to be configured.</param>
        /// <param name="configure">A delegate which is run to configure the services.</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureThrottling(
            [NotNull] this IServiceCollection serviceCollection,
            [NotNull] Action<ThrottlingOptions> configure)
        {
            return serviceCollection.Configure(configure);
        }

        /// <summary>
        /// Add services needed to support throttling to the given <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which Throttling services are added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddThrottling(this IServiceCollection serviceCollection)
        {
            OptionsServiceCollectionExtensions.AddOptions(serviceCollection);
            ServiceCollectionExtensions.AddTransient<IThrottlingPolicyProvider, DefaultThrottlingPolicyProvider>(serviceCollection);
            ServiceCollectionExtensions.AddTransient<IRateStore, InMemoryRateStore>(serviceCollection);
            ServiceCollectionExtensions.AddTransient<ISystemClock, SystemClock>(serviceCollection);
            ServiceCollectionExtensions.AddTransient<IConfigureOptions<ThrottlingOptions>, ThrottlingOptionsSetup>(serviceCollection);
            ServiceCollectionExtensions.AddTransient<IThrottlingService, ThrottlingService>(serviceCollection);
            return serviceCollection;
        }
    }
}