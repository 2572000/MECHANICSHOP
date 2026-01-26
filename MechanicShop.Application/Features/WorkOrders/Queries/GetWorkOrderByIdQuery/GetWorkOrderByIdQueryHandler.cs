using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Application.Features.WorkOrders.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery
{
    public class GetWorkOrderByIdQueryHandler(IAppDbContext context,
        ILogger<GetWorkOrderByIdQueryHandler> logger)
        :IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<GetWorkOrderByIdQueryHandler> _logger = logger;

        public async Task<Result<WorkOrderDto>> Handle(GetWorkOrderByIdQuery request, CancellationToken ct)
        {
            var workOrder = await _context.Workorders
                .AsNoTracking()
                .Include(w => w.RepairTasks).ThenInclude(rt => rt.Parts)
                .Include(w => w.Labor)
                .Include(w => w.Vehicle).ThenInclude(v => v.Customer)
                .Include(w => w.Invoice)
                .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, ct);

            if (workOrder is null)
            {
                _logger.LogWarning("WorkOrder with id {WorkOrderId} was not found", request.WorkOrderId);
                return ApplicationErrors.WorkOrderNotFound;
            }

            return workOrder.ToDto();
        }
    }
}
