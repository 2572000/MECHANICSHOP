using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.Workorders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder
{
    public class CreateWorkOrderCommandHandler(IAppDbContext context,
        ILogger<CreateWorkOrderCommandHandler> logger,
        IWorkOrderPolicy workOrderPolicy,HybridCache cache)
        :IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<CreateWorkOrderCommandHandler> _logger = logger;
        private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;
        private readonly HybridCache _cache = cache;

        public async Task<Result<WorkOrderDto>> Handle(CreateWorkOrderCommand request, CancellationToken ct)
        {
            var repairTasks = await _context.RepairTasks
                .Where(t => request.RepairTaskIds.Contains(t.Id))
                .ToListAsync(ct);

            if (repairTasks.Count != request.RepairTaskIds.Count)
            {
                var missingIds = request.RepairTaskIds.Except(repairTasks.Select(t => t.Id)).ToArray();

                _logger.LogError("Some RepairTaskIds not found: {MissingIds}", string.Join(", ", missingIds));

                return ApplicationErrors.RepairTaskNotFound;
            }

            var totalEstimatedDuration = TimeSpan.FromMinutes(repairTasks.Sum(r => (int)r.EstimatedDurationInMins));
            var endAt = request.StartAt.Add(totalEstimatedDuration);

            if (_workOrderPolicy.IsOutsideOperatingHours(request.StartAt, totalEstimatedDuration))
            {
                _logger.LogError("The WorkOrder time ({StartAt} ? {EndAt}) is outside of store operating hours.", request.StartAt, endAt);
                return ApplicationErrors.WorkOrderOutsideOperatingHour(request.StartAt, endAt);
            }

            var checkMinRequirementResult = _workOrderPolicy.ValidateMinimumRequirement(request.StartAt, endAt);

            if (checkMinRequirementResult.IsError)
            {
                _logger.LogError("WorkOrder duration is shorter than the configured minimum.");

                return checkMinRequirementResult.Errors!;
            }

            var checkSpotAvailabilityResult = await _workOrderPolicy
                .CheckSpotAvailabilityAsync(request.Spot, request.StartAt, endAt, excludeWorkOrderId:null, ct);

            if (checkSpotAvailabilityResult.IsError)
            {
                _logger.LogError("Spot: {Spot} is not available.", request.Spot.ToString());
                return checkSpotAvailabilityResult.Errors!;
            }

            var vehicle = await _context.Vehicles.Include(v => v.Customer)
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId, ct);

            if (vehicle is null)
            {
                _logger.LogError("Vehicle with Id '{VehicleId}' does not exist.", request.VehicleId);

                return ApplicationErrors.VehicleNotFound;
            }

            var labor=await _context.Employees.FindAsync([request.LaborId], ct);
            if (labor is null)
            {
                _logger.LogError("Labor with Id '{LaborId}' does not exist.", request.LaborId!.ToString());
                return ApplicationErrors.LaborNotFound;
            }

            var hasVehicleConflict = await _context.Workorders
           .AnyAsync(
               a =>
               a.VehicleId == request.VehicleId &&
               a.StartAtUtc.Date == request.StartAt.Date &&
               a.StartAtUtc < endAt &&
               a.EndAtUtc > request.StartAt,
               ct);

            if (hasVehicleConflict)
            {
                _logger.LogError("Vehicle with Id '{VehicleId}' already has an overlapping WorkOrder.", request.VehicleId);
                return Error.Conflict(
                    code: "Vehicle_Overlapping_WorkOrders",
                    description: "The vehicle already has an overlapping WorkOrder.");
            }

            var isLaborOccupied = await _context.Workorders
          .AnyAsync(
              a =>
              a.LaborId == request.LaborId &&
              a.StartAtUtc < endAt &&
              a.EndAtUtc > request.StartAt,
              ct);

            if (isLaborOccupied)
            {
                _logger.LogError("Labor with Id '{LaborId}' is already occupied during the requested time.", request.LaborId);
                return Error.Conflict(
                    code: "Labor_Occupied",
                    description: "Labor is already occupied during the requested time.");
            }

            var createWorkOrderResult = Workorder.Create
                (
                Guid.NewGuid(),
                request.VehicleId,
                request.LaborId!.Value,
                request.StartAt,
                endAt,
                request.Spot,
                repairTasks
                );

            if (createWorkOrderResult.IsError)
            {
                _logger.LogError("Failed to create WorkOrder: {Error}", createWorkOrderResult.TopError.Description);
                return createWorkOrderResult.Errors!;
            }

            var workOrder = createWorkOrderResult.Value;
            await _context.Workorders.AddAsync(workOrder, ct);
            workOrder.AddDomainEvent(new WorkOrderCollectionModified());
            await _context.SaveChangesAsync(ct);
            
            workOrder.Vehicle = vehicle;
            workOrder.Labor = labor;
            
            _logger.LogInformation("WorkOrder with Id '{WorkOrderId}' created successfully.", workOrder.Id);

            await _cache.RemoveByTagAsync("work-order", ct);

            return workOrder.ToDto();
        }           
    }
}
