using MechanicShop.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Request.WorkOrder
{
    public class RelocateWorkOrderRequest
    {
        [Required(ErrorMessage = "Spot is required.")]
        [Range(0, 3, ErrorMessage = "Invalid range [0, 1, 2 or 3]")]
        public Spot NewSpot { get; set; }

        [Required(ErrorMessage = "StartAt is required.")]
        public DateTimeOffset NewStartAtUtc { get; set; }
    }
}
