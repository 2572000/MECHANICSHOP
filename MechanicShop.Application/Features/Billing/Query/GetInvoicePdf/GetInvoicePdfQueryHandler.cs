using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MechanicShop.Application.Features.Billing.Query.GetInvoicePdf
{
    public class GetInvoicePdfQueryHandler(IAppDbContext context,
        Logger<GetInvoicePdfQueryHandler> logger,
        IInvoicePdfGenerator pdfGenerator) :IRequestHandler<GetInvoicePdfQuery, Result<InvoicePdfDto>>
    {
        private readonly IAppDbContext _context = context;
        private readonly Logger<GetInvoicePdfQueryHandler> _logger = logger;
        private readonly IInvoicePdfGenerator _pdfGenerator = pdfGenerator;

        public async Task<Result<InvoicePdfDto>> Handle(GetInvoicePdfQuery request, CancellationToken ct)
        {
            var invoice =await _context.Invoices.AsNoTracking()
                .Include(i=>i.LineItems)
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceId,ct);

            if (invoice is null)
            {
                _logger.LogWarning("Invoice not found. InvoiceId: {InvoiceId}", request.InvoiceId);
                return ApplicationErrors.InvoiceNotFound;
            }
            try
            {
                var pdfBytes = pdfGenerator.Generate(invoice);

                var invoicePdf = new InvoicePdfDto
                {
                    Content = pdfBytes,
                    FileName = $"invoice-{invoice.Id}.pdf"
                };

                return invoicePdf;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate PDF for InvoiceId: {InvoiceId}", request.InvoiceId);
                return Error.Failure("An error occurred while generating the invoice PDF.");
            }
        }
    }

}
