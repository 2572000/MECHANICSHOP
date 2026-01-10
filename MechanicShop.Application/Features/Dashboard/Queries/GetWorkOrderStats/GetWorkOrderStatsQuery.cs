using MechanicShop.Application.Features.Dashboard.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Dashboard.Queries.GetWorkOrderStats
{
    public record GetWorkOrderStatsQuery(DateOnly Date): IRequest<Result<TodayWorkOrderStatsDto>>
    {
    }
}
