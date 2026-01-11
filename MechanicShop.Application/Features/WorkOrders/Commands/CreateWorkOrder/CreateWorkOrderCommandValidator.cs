using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder
{
    public class CreateWorkOrderCommandValidator:AbstractValidator<CreateWorkOrderCommand>
    {
        public CreateWorkOrderCommandValidator()
        {
            RuleFor(w => w.Spot)
                .IsInEnum()
                .WithErrorCode("Spot_Invalid")
                .WithMessage("Spot Must Be A Valid Spot Value. [A,B,C,D]");

            RuleFor(RuleFor => RuleFor.VehicleId)
                .NotEmpty()
                .WithMessage("VehicleId is required.");
            RuleFor(RuleFor => RuleFor.StartAt)
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("StartAt must be in the future.");

            RuleFor(RuleFor => RuleFor.RepairTaskIds)
                .NotEmpty()
                .WithMessage("At least one repair task must be selected");

            RuleFor(RuleFor => RuleFor.LaborId)
                .Must(laborId => laborId is null || laborId != Guid.Empty)
                .WithMessage("If provided, LaborId must not be empty.");

        }
    }
}
