using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.ViewModels.DashBoard
{
    public class DashboardStatsVM
    {


        // Customer count
        public int TotalCustomers { get; set; }

        // Total order count
        public int TotalOrders { get; set; }

        // Order status breakdown
        public int PendingOrders { get; set; }
        public int ApprovedOrders { get; set; }

        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }

        public int DeliveredOrders { get; set; }

        public int CancelledOrders { get; set; }

        // Revenue stats
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }

        // Product total count
        public int TotalProducts { get; set; }
    }
}
