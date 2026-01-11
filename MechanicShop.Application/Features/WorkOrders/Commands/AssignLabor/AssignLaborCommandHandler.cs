using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor
{
    public class AssignLaborCommandHandler(IAppDbContext context,
        ILogger<AssignLaborCommandHandler> logger,
        IWorkOrderPolicy workOrderPolicy,HybridCache cache) 
        : IRequestHandler<AssignLaborCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<AssignLaborCommandHandler> _logger = logger;
        private readonly IWorkOrderPolicy _workOrderPolicy = workOrderPolicy;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Updated>> Handle(AssignLaborCommand request, CancellationToken ct)
        {
            var workOrder =await _context.Workorders.FindAsync([request.WorkOrderId],ct);
            if (workOrder is null)
            {
                _logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }

            var labor = await _context.Employees.FindAsync([request.LaborId], ct);
            if (labor is null)
            {
                _logger.LogError("Invalid LaborId: {LaborId}", request.LaborId);
                return ApplicationErrors.LaborNotFound;
            }

            if(await _workOrderPolicy.IsLaborOccupied(request.LaborId,request.WorkOrderId,workOrder.StartAtUtc, workOrder.EndAtUtc))
            {
                _logger.LogError("Labor with Id '{LaborId}' is already occupied during the requested time.", workOrder.LaborId);
                return ApplicationErrors.LaborOccupied;
            }

            var updateLaborResult= workOrder.UpdateLabor(request.LaborId);

            if(updateLaborResult.IsError)
            {
                foreach (var error in updateLaborResult.Errors!)
                {
                    _logger.LogError("[LaborUpdate] {ErrorCode}: {ErrorDescription}", error.Code, error.Description);
                }
                return updateLaborResult.Errors;
            }

            await _context.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"work-order", ct);

            return Result.Updated;

        }
    }
}
