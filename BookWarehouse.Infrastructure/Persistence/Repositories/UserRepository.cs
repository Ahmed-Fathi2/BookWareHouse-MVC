using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using BookWarehouse.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Infrastructure.Persistence.Repositories
{
    public class UserRepository:GenericRepository<ApplicationUser,string>,IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }
      
    }
}
