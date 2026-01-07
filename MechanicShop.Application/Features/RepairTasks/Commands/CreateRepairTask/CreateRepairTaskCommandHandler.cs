using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask
{
    public sealed class CreateRepairTaskCommandHandler(
        IAppDbContext context,
        ILogger<CreateRepairTaskCommandHandler> logger,
        HybridCache cache) : IRequestHandler<CreateRepairTaskCommand,Result<RepairTaskDto>>
    {
        public async Task<Result<RepairTaskDto>> Handle(CreateRepairTaskCommand request, CancellationToken ct)
        {
            var nameExists = await context
                .RepairTasks
                .AnyAsync(p => EF.Functions.Like(p.Name,request.Name), ct);

            if (nameExists)
            {
                logger.LogWarning("Duplicate repair task name '{RepairTaskName}'.", request.Name);
                return RepairTaskErrors.DuplicateName;
            }
            
            List<Part> parts = [];

            foreach (var p in request.Parts)
            {
                var partResult = Part.Create(Guid.NewGuid(), p.Name, p.Quantity, p.Cost);
                if (partResult.IsError)
                {
                    return partResult.Errors!;
                }
                parts.Add(partResult.Value);
            }

            var createRepairTaskResult = RepairTask.Create(
                id: Guid.NewGuid(),
                name: request.Name!,
                laborCost: request.LaborCost,
                estimatedDurationInMins: request.EstimatedDurationInMins!.Value,
                parts: parts
                );

            if (createRepairTaskResult.IsError)
            {
                return createRepairTaskResult.Errors!;
            }

            var repairTask = createRepairTaskResult.Value;

            context.RepairTasks.Add(repairTask);

            await context.SaveChangesAsync(ct);

            logger.LogInformation("Repair task '{RepairTaskName}' was added successfully",repairTask.Name);

            await cache.RemoveByTagAsync("repair-task", ct);

            return repairTask.ToDto();
        }
    }
}
