using BookWarehouse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
           builder.HasKey(p => p.Id);


            builder.Property(p => p.Title)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(1024)
                .IsRequired();

            builder.Property(p => p.ISBN)
               .HasMaxLength(256)
               .IsRequired();

            builder.Property(p => p.Author)
              .HasMaxLength(256)
              .IsRequired();

            builder.Property(p => p.ListPrice)
           .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Price50)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Price100)
                   .HasColumnType("decimal(18,2)");


            builder.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
