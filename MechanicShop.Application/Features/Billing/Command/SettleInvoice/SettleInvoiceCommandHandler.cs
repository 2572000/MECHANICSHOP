using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Billing;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Command.SettleInvoice
{
    public class SettleInvoiceCommandHandler(
        IAppDbContext context,
        ILogger<SettleInvoiceCommandHandler> logger,
        HybridCache cache,
        TimeProvider datetime
        ):IRequestHandler<SettleInvoiceCommand, Result<Success>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<SettleInvoiceCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;
        private readonly TimeProvider _datetime = datetime;

        public async Task<Result<Success>> Handle(SettleInvoiceCommand request, CancellationToken ct)
        {
            var invoice =await _context.Invoices.FindAsync(request.InvoiceId,ct);
            if (invoice is null)
            {
                _logger.LogWarning("Invoice {InvoiceId} not found.", request.InvoiceId);
                return  ApplicationErrors.InvoiceNotFound;
            }
            var payInvoiceResult = invoice.MarkAsPaid(_datetime);

            if (payInvoiceResult is null)
            {
                _logger.LogWarning("Invoice payment failed for InvoiceId: {InvoiceId}. Errors: {Errors}",
                    invoice.Id,
                   payInvoiceResult!.Errors);
                return payInvoiceResult.Errors!;
            }

            await _context.SaveChangesAsync(ct);

            await _cache.RemoveByTagAsync("invoice", ct);

            _logger.LogInformation("Invoice {InvoiceId} successfully paid.", invoice.Id);

            return Result.Success;
        }
    }
}
