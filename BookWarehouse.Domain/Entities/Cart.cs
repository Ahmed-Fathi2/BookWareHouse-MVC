using BookWarehouse.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookWarehouse.Domain.Entities
{
    public class Cart : IAuditableEntity
    {
        public Guid Id { get; set; }

        public int Count { get; set; }

      


        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string ApplicationUserId { get; set; }= string.Empty;

        public ApplicationUser ApplicationUser { get; set; }=default!;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
