using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Query.GetInvoiceById
{
    public class GetInvoiceByIdQueryHandler(IAppDbContext context,
        Logger<GetInvoiceByIdQueryHandler> logger
        ):IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly Logger<GetInvoiceByIdQueryHandler> _logger = logger;

        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
        {
            var invoice = await _context.Invoices
                .Include(i => i.LineItems)
                .Include(i => i.WorkOrder!)
                    .ThenInclude(w => w.Vehicle!)
                        .ThenInclude(v => v.Customer)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, ct);

            if (invoice is null)
            {
                _logger.LogWarning("Invoice not found. InvoiceId: {InvoiceId}", request.InvoiceId);
                return ApplicationErrors.InvoiceNotFound;
            }

            return invoice.ToDto();
        }
    }
}
