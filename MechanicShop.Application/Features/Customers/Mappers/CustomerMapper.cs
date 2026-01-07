using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;

namespace MechanicShop.Application.Features.Customers.Mappers
{
    public static class CustomerMapper
    {
        //Convert from Customer to CustomerDto  

        public static CustomerDto ToDto(this Customer entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity)); 

            return new CustomerDto
            {
                CustomerId = entity.Id,
                Email = entity.Email!,
                Name = entity.Name!,
                PhoneNumber = entity.PhoneNumber!,
                Vehicles = entity.Vehicles?.Select(v => v.ToDto()).ToList() ?? []
            };
        }

        public static List<CustomerDto> ToDtos(this IEnumerable<Customer> entities)
        {
            return [..entities.Select(c => c.ToDto())];
        }

        public static VehicleDto ToDto(this Vehicle entity)
        {
            ArgumentException.ThrowIfNullOrEmpty(nameof(entity));

            return new VehicleDto
            (
                 entity.Id,
                 entity.Make,
                entity.Model,
                 entity.Year,
                 entity.LicensePlate
            );
        }

        public static List<VehicleDto> ToDtos(this IEnumerable<Vehicle> entities)
        {
            return [.. entities.Select(c => c.ToDto())];
        }
    }
}
