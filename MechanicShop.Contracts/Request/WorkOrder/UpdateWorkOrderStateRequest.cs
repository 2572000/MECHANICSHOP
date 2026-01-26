using MechanicShop.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Request.WorkOrder
{
    public class UpdateWorkOrderStateRequest
    {
        public WorkOrderState State { get; set; }
    }
}
