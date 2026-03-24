using BookWarehouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Entities
{
    public class Product:IAuditableEntity
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; }= string.Empty;


        public string ISBN { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;


        public decimal ListPrice { get; set; } // Price Before Discount

        public decimal Price { get; set; } // Price for 1-50 units

        public decimal Price50 { get; set; } // Price for 51-100 units

        public decimal Price100 { get; set; } // Price for more than 100  units


        public string ImageUrl { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;


        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;


        //public ICollection<ProductImage> Products { get; set; } = new HashSet<ProductImage>();
    }
}


