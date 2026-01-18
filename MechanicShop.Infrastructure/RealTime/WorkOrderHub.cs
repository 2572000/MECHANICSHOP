using Microsoft.AspNetCore.SignalR;

namespace MechanicShop.Infrastructure.RealTime
{
    public class WorkOrderHub:Hub
    {
        public const string HubUrl = "/hubs/workorders";
    }
}
