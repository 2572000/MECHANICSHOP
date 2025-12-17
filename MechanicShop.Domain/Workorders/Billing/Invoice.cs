using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Billing.Enums;

namespace MechanicShop.Domain.Workorders.Billing
{
    public sealed class Invoice: AuditableEntity
    {
        public decimal DiscountAmount { get; private set; }
        public DateTimeOffset IssuedAtUtc { get; }
        public DateTimeOffset? PaidAt { get; private set; }
        public decimal TaxAmount { get; }
        public decimal Subtotal => LineItems.Sum(x => x.LineTotal);
        public decimal Total => Subtotal - DiscountAmount + TaxAmount;
        public Guid WorkOrderId { get; }
        public Workorder? WorkOrder { get; set; } 
        public InvoiceStatus Status { get; private set; }


        private readonly List<InvoiceLineItem> _lineItems = [];
        public IReadOnlyList<InvoiceLineItem> LineItems => _lineItems;


        private Invoice()
        {
            
        }

        private Invoice(Guid id, Guid workOrderId, DateTimeOffset issuedAt, List<InvoiceLineItem> lineItems, decimal discountAmount, decimal taxAmount)
            :base(id)
        {
            WorkOrderId = workOrderId;
            IssuedAtUtc = issuedAt;
            TaxAmount = taxAmount;
            DiscountAmount = discountAmount;
            Status = InvoiceStatus.Unpaid;
            _lineItems = lineItems;
        }

        public static Result<Invoice> Create(
            Guid id, 
            Guid workOrderId, 
            List<InvoiceLineItem> lineItems, 
            decimal discountAmount, 
            decimal taxAmount,
            TimeProvider datetime)
        {
            if (workOrderId == Guid.Empty)
            {
                return InvoiceErrors.WorkOrderIdInvalid;
            }
            if(lineItems==null ||  lineItems.Count == 0)
            {
                return InvoiceErrors.LineItemsEmpty;
            }
            return new Invoice(id,workOrderId,datetime.GetUtcNow(), lineItems, discountAmount, taxAmount);
        }

        public Result<Updated> ApplayDiscount(decimal discountAmount)
        {
            if (Status == InvoiceStatus.Paid)
            {
                return InvoiceErrors.InvoiceLocked;
            }
            if (discountAmount < 0 )
            {
                return InvoiceErrors.DiscountNegative;
            }
            if (discountAmount > Subtotal)
            {
                return InvoiceErrors.DiscountExceedsSubtotal;
            }

            DiscountAmount = discountAmount;

            return Result.Updated;
        }

        public Result<Updated> MarkAsPaid(TimeProvider datetime)
        {
            if (Status == InvoiceStatus.Paid)
            {
                return InvoiceErrors.InvoiceLocked;
            }
            Status = InvoiceStatus.Paid;
            PaidAt = datetime.GetUtcNow();
            return Result.Updated;
        }

    }
}
