using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer
{
    public record CreateVehicleCommand(
        string Make,
        string Model,
        string LicensePlate,
        int Year):IRequest<Result<VehicleDto>>;
    
}