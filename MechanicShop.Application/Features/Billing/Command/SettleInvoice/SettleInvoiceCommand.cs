using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Billing.Command.SettleInvoice
{
    public sealed record SettleInvoiceCommand(Guid InvoiceId) : IRequest<Result<Success>>
    {
    }
}
