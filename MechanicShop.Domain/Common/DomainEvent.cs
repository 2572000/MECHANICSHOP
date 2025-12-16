
using MediatR;   //Package: MediatR

namespace MechanicShop.Domain.Common
{
    // Base class for all domain events in the system
    // Implements MediatR's INotification, so it can be published to handlers
    public abstract class DomainEvent : INotification
    {
        // This class is intentionally empty as a marker
        // All specific domain events will inherit from this class
    }
}
