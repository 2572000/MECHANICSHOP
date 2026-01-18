using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Workorders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.EventHandlers
{
    public class SendWorkOrderCompletedEmailHandler(IAppDbContext context,
        ILogger<SendWorkOrderCompletedEmailHandler> logger,
        INotificationService notificationService)
        :INotificationHandler<WorkOrderCompleted>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<SendWorkOrderCompletedEmailHandler> _logger = logger;
        private readonly INotificationService _notificationService = notificationService;

        public async Task Handle(WorkOrderCompleted notification, CancellationToken ct)
        {
            var workOrder = await _context.Workorders
                       .Include(w => w.Vehicle!).ThenInclude(v => v.Customer)
                       .AsNoTracking()
                       .FirstOrDefaultAsync(w => w.Id == notification.WorkOrderId, ct);

            if (workOrder is null)
            {
                _logger.LogError("WorkOrder with Id '{WorkOrderId}' does not exist.", notification.WorkOrderId);
                return;
            }

            var emailSubject = "Vehicle Service Complete";
            var emailBody = $"<h1>Hello {workOrder.Vehicle?.Customer?.Name}</h1>" +
                            $"<p>Your vehicle service for {workOrder.Vehicle?.LicensePlate} is complete. You may collect it from the shop.</p>";

            var smsMessage = $"Hello {workOrder.Vehicle?.Customer?.Name}, your vehicle service for {workOrder.Vehicle?.LicensePlate} is complete.";



            await _notificationService.SendEmailAsync(workOrder.Vehicle?.Customer?.Email!, emailSubject, emailBody, ct);
            await _notificationService.SendSmsAsync(workOrder.Vehicle?.Customer?.PhoneNumber!, smsMessage, ct);
        }
    }
}
