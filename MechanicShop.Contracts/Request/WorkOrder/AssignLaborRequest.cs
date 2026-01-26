using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Request.WorkOrder
{
    public class AssignLaborRequest
    {
        [Required(ErrorMessage = "LaborId is required.")]
        public string LaborId { get; set; } = string.Empty;
    }
}
