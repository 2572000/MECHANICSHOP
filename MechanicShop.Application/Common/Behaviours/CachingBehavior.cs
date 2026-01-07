using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results.Abstractions;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Common.Behaviours
{
    // MediatR pipeline behavior responsible for caching query results
    // Executes before and after the actual request handler
    public class CachingBehavior<TRequest, TResponse>(
        HybridCache cache,
        ILogger<CachingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly HybridCache _cache = cache;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger = logger;

        // Main pipeline execution method
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            // If the request does NOT implement ICachedQuery,
            // skip caching and execute the handler directly
            if (request is not ICachedQuery cachedRequest)
                return await next(ct);

            _logger.LogInformation("Checking cache for {RequestName}",typeof(TRequest).Name);

            // Try to get the value from cache using the cache key
            var result = await _cache.GetOrCreateAsync<TResponse>(
                cachedRequest.CacheKey,
                // We do NOT create a value here
                // Return null if the entry does not exist
                _ => new ValueTask<TResponse>((TResponse)(object)null!),

                // Cache entry options
                new HybridCacheEntryOptions
                {
                    // Prevent fallback to an underlying data source
                    Flags = HybridCacheEntryFlags.DisableUnderlyingData
                },
                cancellationToken: ct);

            // If the value was not found in cache
            if (result is null)
            {
                // Execute the actual request handler (DB / external service)
                result = await next(ct);

                // Cache only successful results
                if (result is IResult res && res.IsSuccess)
                {
                    _logger.LogInformation("Caching result for {RequestName}",typeof(TRequest).Name);

                    // Store the result in cache
                    await _cache.SetAsync(
                        cachedRequest.CacheKey,
                        result,
                        new HybridCacheEntryOptions
                        {
                            // Cache expiration duration
                            Expiration = cachedRequest.Expiration
                        },
                        // Cache tags for grouped invalidation
                        cachedRequest.Tags,
                        ct);
                }
            }

            // Return the result (from cache or handler)
            return result;
        }
    }

}
