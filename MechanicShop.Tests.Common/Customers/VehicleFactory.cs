using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Tests.Common.Customers
{
    public static class VehicleFactory
    {
        public static Result<Vehicle> CreateVehicle(Guid? id = null,
                                                    string? make = null,
                                                    string? model = null,
                                                    int? year = null,
                                                    string? licensePlate = null)
        {
           return Vehicle.Create(
           id ?? Guid.NewGuid(),
           make ?? "Honda",
           model ?? "Accord",
           licensePlate ?? "ABC 123",
           year ?? 2024
           );
        }
    }
}
