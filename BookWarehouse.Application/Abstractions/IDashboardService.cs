using BookWarehouse.Application.ViewModels.DashBoard;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Abstractions
{
    public interface IDashboardService
    {
        Task<DashboardStatsVM> GetDashboardStatsAsync();

        Task<IEnumerable<CustomerDetailsVM>> GetCustomerDetailsAsync();
    }
}
