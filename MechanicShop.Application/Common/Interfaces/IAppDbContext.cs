using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Identity;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.Workorders.Billing;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Common.Interfaces
{
    //When You Sure You Will Use Entity Framework Core In Your Application
    public interface IAppDbContext
    {
        public DbSet<Employee> Employees { get;  }
        public DbSet<Customer> Customers { get;  }
        public DbSet<Vehicle> Vehicles { get;  }
        public DbSet<RepairTask> RepairTasks { get;  }
        public DbSet<Part> Parts { get;  }
        public DbSet<Workorder> Workorders { get;  }
        public DbSet<Invoice> Invoices { get;  }
        public DbSet<RefreshToken> RefreshTokens { get;  }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
