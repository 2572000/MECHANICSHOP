using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery:ICachedQuery<Result<CustomerDto>>
    {
        public Guid CustomerId { get; }

        public string CacheKey => $"customer_{CustomerId}";

        public string[] Tags => ["customer"];

        public TimeSpan Expiration => TimeSpan.FromMinutes(10);
    }
}
