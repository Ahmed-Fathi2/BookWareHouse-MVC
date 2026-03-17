using BookWarehouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Domain.Repositories
{
    public interface ICategoryRepository:IGenericRepository<Category,Guid>
    {
    }
}
