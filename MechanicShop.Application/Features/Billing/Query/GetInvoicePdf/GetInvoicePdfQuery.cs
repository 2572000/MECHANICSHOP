using MechanicShop.Application.Features.Billing.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Billing.Query.GetInvoicePdf
{
    public record GetInvoicePdfQuery(Guid InvoiceId) : IRequest<Result<InvoicePdfDto>>;
}
