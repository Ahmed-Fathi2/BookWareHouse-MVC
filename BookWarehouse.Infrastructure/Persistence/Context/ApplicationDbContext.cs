using BookWarehouse.Domain.Common.BaseEntity;
using BookWarehouse.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BookWarehouse.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext() { }


        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>(); // Local Container for Category entities --->> To Prevent Null Reference Exception
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetails> OrderDetails => Set<OrderDetails>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = true;
                        entry.State = EntityState.Modified; // Soft delete
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}