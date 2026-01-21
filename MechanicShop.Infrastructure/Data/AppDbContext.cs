using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Identity;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.Workorders.Billing;
using MechanicShop.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Infrastructure.Data
{

    public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext
    {
        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }


        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<RepairTask> RepairTasks => Set<RepairTask>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        public DbSet<Workorder> Workorders => Set<Workorder>();

        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (_mediator != null)
                await DispatchDomainEventsAsync(cancellationToken);

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEntities = ChangeTracker.Entries()
                .Where(e => e.Entity is Entity baseEntity && baseEntity.DomainEvents.Count != 0)
                .Select(e => (Entity)e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            foreach (var entity in domainEntities)
            {
                entity.ClearDomainEvent();
            }
        }
    }
}
