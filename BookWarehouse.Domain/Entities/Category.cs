using BookWarehouse.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Entities
{
    public class Category:IAuditableEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }=string.Empty;

        public int DisplayOrder { get; set; }


        public DateTime CreatedAt {  get; set; }=DateTime.UtcNow;
        public DateTime? UpdatedAt {  get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
