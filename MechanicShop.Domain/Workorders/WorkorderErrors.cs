using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Enums;

namespace MechanicShop.Domain.Workorders
{
    public static class WorkorderErrors
    {
        public static Error WorkOrderIdRequired =>
            Error.Validation("WorkOrderErrors.WorkOrderIdRequired", "WorkOrder Id is required");

        public static Error VehicleIdRequired => 
            Error.Validation("WorkOrderErrors.VehicleIdRequired","Vehicle Id is required");

        public static Error RepairTasksRequired => 
            Error.Validation("WorkOrderErrors.RepairTasksRequired","At least one repair task is required");

        public static Error LaborIdRequired => 
            Error.Validation( "WorkOrderErrors.LaborIdRequired", "Labor Id is required");

        public static Error InvalidTiming =>
            Error.Conflict("WorkOrderErrors.InvalidTiming","End time must be after start time.");

        public static Error SpotInvalid => 
            Error.Validation("WorkOrderErrors.SpotInvalid","The provided spot is invalid");

        public static Error Readonly => 
            Error.Conflict("WorkOrderErrors.Readonly","WorkOrder is read-only.");

        public static Error TimingReadonly(string id, WorkOrderState state) => 
            Error.Conflict("WorkOrderErrors.TimingReadonly",
                $"WorkOrder '{id}': Can't Modify timing when WorkOrder status is '{state}'.");


        public static Error LaborIdEmpty(string id) => 
            Error.Validation("WorkOrderErrors.LaborIdEmpty",$"WorkOrder '{id}': Labor Id is empty");

        public static Error StateTransitionNotAllowed(DateTimeOffset startAtUtc) 
            => Error.Conflict( "WorkOrderErrors.StateTransitionNotAllowed",
           $"State transition is not allowed before the work order’s scheduled start time {startAtUtc:yyyy-MM-dd HH:mm} UTC.");

        public static Error InvalidStateTransition(WorkOrderState current, WorkOrderState next) 
            => Error.Conflict("WorkOrderErrors.InvalidStateTransition",
            $"WorkOrder Invalid State transition from '{current}' to '{next}'.");

        public static Error RepairTaskAlreadyAdded =>
            Error.Conflict("WorkOrderErrors.RepairTaskAlreadyAdded","Repair task already exists.");

        public static Error InvalidStateTransitionTime
            => Error.Conflict("WorkOrderErrors.InvalidStateTransitionTime",
           "State transition is not allowed before the work order’s scheduled start time.");
    }
}
