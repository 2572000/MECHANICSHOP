using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask
{
    public class UpdateRepairTaskCommandHandler(IAppDbContext context,
        ILogger<UpdateRepairTaskCommandHandler> logger,HybridCache cache)
        :IRequestHandler<UpdateRepairTaskCommand,Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateRepairTaskCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Updated>> Handle(UpdateRepairTaskCommand request, CancellationToken ct)
        {
            var repairTask = await _context.RepairTasks
                .Include(r=>r.Parts)
                .FirstOrDefaultAsync(r=>r.Id==request.RepairTaskId, ct);

            if(repairTask is null)
            {
                _logger.LogWarning("RepairTask {RepairTaskId} not found for update.", request.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }

            var validatedParts = new List<Part>();

            foreach (var p in request.Parts)
            {
                var partId = p.PartId ?? Guid.NewGuid();

                var partResult = Part.Create(partId, p.Name,  p.Quantity, p.Cost);

                if (partResult.IsError)
                {
                    return partResult.Errors!;
                }

                validatedParts.Add(partResult.Value);
            }

            var updateRepairTaskResult=repairTask.Update(request.Name, request.LaborCost, request.EstimatedDurationInMins);
            if (updateRepairTaskResult is null)
            {
                return updateRepairTaskResult.Errors!;
            }

            var upsertPartsResult=repairTask.UpsertParts(validatedParts);

            if (upsertPartsResult.IsError)
            {
                return upsertPartsResult.Errors!;
            }

            await _context.SaveChangesAsync(ct);


            await _cache.RemoveByTagAsync("repair-task", ct);

            return Result.Updated;
        }
    }
}
