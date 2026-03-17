using System.Reflection.Metadata.Ecma335;

namespace BookWarehouse.Domain.Common
{
    public interface IAuditableEntity
    {

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; }

        //public string CreatedBy { get; set; }
        //public string UpdatedBy { get; set; }
        //public string DeletedBy { get; set; }



    }
}
