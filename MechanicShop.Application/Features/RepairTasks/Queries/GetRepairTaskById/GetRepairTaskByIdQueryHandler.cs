using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Dtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById
{
    public sealed class GetRepairTaskByIdQueryHandler(IAppDbContext context,
        ILogger<GetRepairTaskByIdQueryHandler> logger)
        :IRequestHandler<GetRepairTaskByIdQuery, Result<RepairTaskDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetRepairTaskByIdQueryHandler> _logger = logger;

        public async Task<Result<RepairTaskDto>> Handle(GetRepairTaskByIdQuery request, CancellationToken ct)
        {
            var repairTask=await _context.RepairTasks.Include(p=>p.Parts).AsNoTracking()
                .FirstOrDefaultAsync(p=>p.Id==request.RepairTaskId,ct);
            if (repairTask is null)
            {
                _logger.LogWarning("Repair Task with Id {RepairTaskId} was not found.", request.RepairTaskId);
                return ApplicationErrors.RepairTaskNotFound;
            }
            return repairTask.ToDto();
        }
    }
}
