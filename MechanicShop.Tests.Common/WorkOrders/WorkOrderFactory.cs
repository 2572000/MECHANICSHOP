using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.Workorders.Enums;
using MechanicShop.Tests.Common.RepaireTasks;

namespace MechanicShop.Tests.Common.WorkOrders
{
    public static class WorkOrderFactory
    {
        public static Result<Workorder> CreateWorkOrder(
      Guid? id = null,
      Guid? vehicleId = null,
      DateTimeOffset? startAt = null,
      DateTimeOffset? endAt = null,
      Guid? laborId = null,
      Spot? spot = null,
      List<RepairTask>? repairTasks = null)
        {
            return Workorder.Create(
                id ?? Guid.NewGuid(),
                vehicleId ?? Guid.NewGuid(),
                laborId ?? Guid.NewGuid(),
                startAt ?? DateTimeOffset.UtcNow,
                endAt ?? DateTimeOffset.UtcNow.AddHours(1),
                spot ?? Spot.A,
                repairTasks ?? [RepairTaskFactory.CreateRepairTask().Value]);
        }
    }
}
