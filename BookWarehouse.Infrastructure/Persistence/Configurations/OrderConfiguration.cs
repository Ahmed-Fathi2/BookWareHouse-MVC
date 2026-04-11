using BookWarehouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookWarehouse.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
  

            builder.Property(x => x.FullName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

            builder.Property(x => x.StreetAddress)
                .HasMaxLength(512)
                .IsRequired();

            builder.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.TrackingNumber)
                .HasMaxLength(256);

            builder.Property(x => x.Carrier)
              .HasMaxLength(256);



            builder.Property(x=>x.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(50);


            builder.Property(x => x.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);


            builder.HasOne(x => x.ApplicationUser)
                   .WithMany(ApplicationUser => ApplicationUser.Orders)
                   .HasForeignKey(x => x.ApplicationUserId);

        }
}
}











