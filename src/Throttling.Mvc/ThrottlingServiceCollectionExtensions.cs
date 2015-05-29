using Throttling.Mvc;

namespace Microsoft.Framework.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> extensions for enabling Throttling support.
    /// </summary>
    public static class ThrottlingServiceCollectionExtensions
    {
        /// <summary>
        /// Add services needed to support throttling to the given <paramref name="serviceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The service collection to which Throttling services are added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddMvcThrottling(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddThrottling();
            serviceCollection.AddTransient<IThrottlingFilter, ThrottlingFilter>();

            return serviceCollection;
        }
    }
}