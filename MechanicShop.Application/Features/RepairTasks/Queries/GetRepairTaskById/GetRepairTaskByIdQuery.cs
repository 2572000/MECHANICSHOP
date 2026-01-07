using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public sealed record GetRepairTaskByIdQuery(Guid RepairTaskId) : ICachedQuery<Result<RepairTaskDto>>
    {
        public string CacheKey => $"repair_task_{RepairTaskId}";

        public string[] Tags => ["repair-task"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
