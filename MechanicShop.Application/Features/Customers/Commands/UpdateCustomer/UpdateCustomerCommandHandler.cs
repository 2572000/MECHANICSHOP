using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler(IAppDbContext context,
        ILogger<UpdateCustomerCommandHandler> logger,
        HybridCache cache):IRequestHandler<UpdateCustomerCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
        private readonly HybridCache _cache = cache;

        public async Task<Result<Updated>> Handle(UpdateCustomerCommand request, CancellationToken ct)
        {
            var customer = await _context.Customers.Include(c => c.Vehicles)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId, ct);

            if (customer is null)
            {
                _logger.LogWarning("Customer {CustomerId} not found for update.", request.CustomerId);
                return ApplicationErrors.CustomerNotFound;
            }

            var validatedVehicles = new List<Vehicle>();

            foreach(var v in request.Vehicles)
            {
                var vehicleId = v.VehicleId ?? Guid.NewGuid();
                var vehicleResult = Vehicle.Create(vehicleId, v.Make, v.Model, v.LicensePlate, v.Year);
                if (vehicleResult.IsError)
                {
                    return vehicleResult.Errors!;
                }
                validatedVehicles.Add(vehicleResult.Value);
            }

            var updateCustomerResult = customer.Update(request.Name, request.Email, request.PhoneNumber);

            if (updateCustomerResult.IsError)
            {
                return updateCustomerResult.Errors!;
            }

            var upsertVehiclesResult = customer.UpsertParts(validatedVehicles);

            if (upsertVehiclesResult.IsError)
            {
                return upsertVehiclesResult.Errors!;
            }

            await _context.SaveChangesAsync(ct);

            await _cache.RemoveAsync($"customer", ct);
            return Result.Updated;
        }
    }
}
