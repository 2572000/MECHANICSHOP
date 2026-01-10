using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MechanicShop.Application.Features.RepairTasks.Commands.RemoveRepairTask
{
    public class RemoveRepairTaskCommandHandler(IAppDbContext context,
        ILogger<RemoveRepairTaskCommandHandler> logger,
        HybridCache cache)
        :IRequestHandler<RemoveRepairTaskCommand,Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<RemoveRepairTaskCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Deleted>> Handle(RemoveRepairTaskCommand request, CancellationToken ct)
        {
            var repairTask =await _context.RepairTasks
                .FindAsync([request.RepairTaskId], ct);

            if (repairTask is null)
            {
                _logger.LogWarning("RepairTask {RepairTaskId} not found for deletion.", request.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }

            var isInUse = await _context.Workorders.AsNoTracking()
                .SelectMany(w => w.RepairTasks)
                .AnyAsync(rt => rt.Id == request.RepairTaskId, ct);

            if (isInUse)
            {
                _logger.LogWarning("RepairTask {RepairTaskId} cannot be deleted — in use by work orders.", request.RepairTaskId);
                return RepairTaskErrors.InUse;
            }

            _context.RepairTasks.Remove(repairTask);
            await _context.SaveChangesAsync(ct);

            await _cache.RemoveByTagAsync("repair-task", ct);
            _logger.LogInformation("RepairTask {RepairTaskId} deleted successfully.", request.RepairTaskId);
            return Result.Deleted;
        }
    }
}
