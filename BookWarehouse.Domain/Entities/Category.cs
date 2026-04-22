using BookWarehouse.Domain.Common.BaseEntity;

namespace BookWarehouse.Domain.Entities
{
    public class Category : IAuditableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string? ImageUrl { get; set; } 

        public int DisplayOrder { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;



        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
