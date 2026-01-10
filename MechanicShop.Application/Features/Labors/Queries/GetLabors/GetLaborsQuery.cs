using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Labors.Queries.GetLabors
{
    public record GetLaborsQuery() : ICachedQuery<Result<List<LaborDto>>>
    {
        public string CacheKey => $"Labors";

        public string[] Tags => ["Labors"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
