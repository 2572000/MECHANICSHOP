using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.Workorders.Billing;
using MechanicShop.Domain.Workorders.Enums;
namespace MechanicShop.Domain.Workorders
{
    public sealed class Workorder : AuditableEntity
    {
        public DateTimeOffset StartAtUtc { get; private set; }
        public DateTimeOffset EndAtUtc { get; private set; }
        public Spot Spot { get; private set; }
        public WorkOrderState State { get; private set; }
        public Guid LaborId { get; private set; }
        public Employee? Labor { get; set; }
        public Guid VehicleId { get; }
        public Vehicle? Vehicle { get; set; }
        public Invoice? Invoice { get; set; }
        public decimal? Discount { get; private set; }
        public decimal? Tax { get; private set; }
        public decimal? TotalPartsCost => _repairTasks.SelectMany(rt => rt.Parts).Sum(p => p.Cost);
        public decimal? TotalLaborCost => _repairTasks.Sum(rt => rt.LaborCost);
        public decimal? Total => (TotalPartsCost ?? 0) + (TotalLaborCost ?? 0);

        private readonly List<RepairTask> _repairTasks = [];
        public IEnumerable<RepairTask> RepairTasks => _repairTasks.AsReadOnly();

        public bool IsEditable => State is not (WorkOrderState.Completed or WorkOrderState.Cancelled or WorkOrderState.InProgress);

        private Workorder()
        {

        }

        private Workorder(
            Guid id,
            Guid laborId, Guid vehicleId, DateTimeOffset startAt, DateTimeOffset endAt,
            Spot spot, WorkOrderState state, List<RepairTask> repairTasks)
            : base(id)
        {
            VehicleId = vehicleId;
            StartAtUtc = startAt;
            EndAtUtc = endAt;
            LaborId = laborId;
            Spot = spot;
            State = state;
            _repairTasks = repairTasks;
        }

        public static Result<Workorder> Create(
            Guid id,
            Guid vehicleId, Guid laborId,
            DateTimeOffset startAt, DateTimeOffset endAt,
            Spot spot, List<RepairTask> repairTasks)
        {
            if (id == Guid.Empty)
            {
                return WorkorderErrors.WorkOrderIdRequired;
            }
            if (vehicleId == Guid.Empty)
            {
                return WorkorderErrors.VehicleIdRequired;
            }
            if (laborId == Guid.Empty)
            {
                return WorkorderErrors.LaborIdRequired;
            }
            if (repairTasks == null || repairTasks.Count == 0)
            {
                return WorkorderErrors.RepairTasksRequired;
            }
            if (endAt <= startAt)
            {
                return WorkorderErrors.InvalidTiming;
            }
            if (!Enum.IsDefined(spot))
            {
                return WorkorderErrors.SpotInvalid;
            }
            return new Workorder(id, laborId, vehicleId, startAt, endAt, spot, WorkOrderState.Scheduled, repairTasks);
        }

        public Result<Updated> AddRepairTask(RepairTask repairTasks)
        {
            if (!IsEditable)
            {
                return WorkorderErrors.Readonly;
            }

            if (_repairTasks.Any(r => r.Id == repairTasks.Id))
            {
                return WorkorderErrors.RepairTaskAlreadyAdded;
            }
            _repairTasks.Add(repairTasks);
            return Result.Updated;
        }

        public Result<Updated> UpdateTiming(DateTimeOffset startAt, DateTimeOffset endAt)
        {
            if (!IsEditable)
            {
                return WorkorderErrors.TimingReadonly(Id.ToString(), State);
            }
            if (endAt <= startAt)
            {
                return WorkorderErrors.InvalidTiming;
            }
            StartAtUtc = startAt;
            EndAtUtc = endAt;
            return Result.Updated;
        }

        public Result<Updated> UpdateLabor(Guid laborId)
        {
            if (!IsEditable)
            {
                return WorkorderErrors.Readonly;
            }
            if (laborId == Guid.Empty)
            {
                return WorkorderErrors.LaborIdEmpty(Id.ToString());
            }
            LaborId = laborId;
            return Result.Updated;
        }


        public Result<Updated> UpdateState(WorkOrderState nextState)
        {
            if (!CanTransitionTo(nextState))
            {
                return WorkorderErrors.InvalidStateTransition(State, nextState);
            }
            State = nextState;
            return Result.Updated;
        }


        public Result<Updated> Cancel()
        {
            if (!CanTransitionTo(WorkOrderState.Cancelled))
            {
                return WorkorderErrors.InvalidStateTransition(State, WorkOrderState.Cancelled);
            }
            State = WorkOrderState.Cancelled;
            return Result.Updated;
        }

        public Result<Updated> ClearRepairTasks()
        {
            if(!IsEditable)
            {
                return WorkorderErrors.Readonly;
            }
            _repairTasks.Clear();
            return Result.Updated;

        }


        public Result<Updated> UpdateSpot(Spot newSpot)
        {
            if(!IsEditable)
            {
                return WorkorderErrors.Readonly;
            }
            if(!Enum.IsDefined(newSpot))
            {
                return WorkorderErrors.SpotInvalid;
            }
            Spot = newSpot;
            return Result.Updated;
        }

        public bool CanTransitionTo(WorkOrderState newStatus)
        {
            return (State, newStatus) switch
            {
                (WorkOrderState.Scheduled, WorkOrderState.InProgress) => true,
                (WorkOrderState.InProgress, WorkOrderState.Completed) => true,
                (_, WorkOrderState.Cancelled) when State != WorkOrderState.Completed => true,
                _ => false
            };
        }
    }
}
