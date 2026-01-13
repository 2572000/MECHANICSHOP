

using MechanicShop.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id).IsClustered(false);


            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Email)
              .IsRequired()
              .HasMaxLength(150);


            builder.Property(c => c.PhoneNumber)
              .IsRequired()
              .HasMaxLength(20);

            builder.HasMany(c => c.Vehicles)
                .WithOne(v => v.Customer)
                .HasForeignKey(v => v.CustomerId);

            builder.Navigation(c => c.Vehicles)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
