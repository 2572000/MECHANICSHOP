using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Workorders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder
{
    public record RelocateWorkOrderCommand(
        Guid WorkOrderId,
        DateTimeOffset NewStartAt,
        Spot NewSpot
    ) : IRequest<Result<Updated>>;
   
}
