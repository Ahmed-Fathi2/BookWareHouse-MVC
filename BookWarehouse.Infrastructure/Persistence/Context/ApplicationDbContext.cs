using BookWarehouse.Domain.Common;
using BookWarehouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext() { }


        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Category> Categories =>Set<Category>(); // Local Container for Category entities --->> To Prevent Null Reference Exception

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        { 
            foreach(var entry in ChangeTracker.Entries<IAuditableEntity>())
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

            return base.SaveChanges();
        }

    }
}