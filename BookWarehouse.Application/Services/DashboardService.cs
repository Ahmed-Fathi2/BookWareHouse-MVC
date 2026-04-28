using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.ViewModels.DashBoard;
using BookWarehouse.Domain.Common.Enums;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookWarehouse.Application.Services
{
    public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<CustomerDetailsVM>> GetCustomerDetailsAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(includes: [x => x.Orders] );
            var customerDetails = users.Select(u => new CustomerDetailsVM
            {
                CustomerId= u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                OrdersCount = u.Orders.Count()
            }).ToList();

            return customerDetails;

        }
        public async Task<DashboardStatsVM> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfNextMonth = startOfMonth.AddMonths(1);

     
            IQueryable<Order> ordersQuery = await _unitOfWork.OrderRepository.GetAllAsync();

            var ordersData = await ordersQuery
                .GroupBy(x => 1)
                .Select(g => new
                {
                    TotalOrders = g.Count(),
                    Pending = g.Count(x => x.OrderStatus == OrderStatus.Pending),
                    Approved = g.Count(x => x.OrderStatus == OrderStatus.Approved),
                    Processing = g.Count(x => x.OrderStatus == OrderStatus.Processing),
                    Shipped = g.Count(x => x.OrderStatus == OrderStatus.Shipped),
                    Delivered = g.Count(x => x.OrderStatus == OrderStatus.Delivered),
                    Cancelled = g.Count(x => x.OrderStatus == OrderStatus.Cancelled),
                    TotalCustomers = g.Select(x => x.ApplicationUserId).Distinct().Count(),
                    TotalRevenue = g.Where(x => x.PaymentStatus == PaymentStatus.Paid)
                                    .Sum(x => (decimal?)x.OrderTotal) ?? 0,
                    TodayRevenue = g.Where(x => x.PaymentStatus == PaymentStatus.Paid
                                && x.OrderDate >= today && x.OrderDate < tomorrow)
                                .Sum(x => (decimal?)x.OrderTotal) ?? 0,
                    MonthlyRevenue = g.Where(x => x.PaymentStatus == PaymentStatus.Paid
                                && x.OrderDate >= startOfMonth && x.OrderDate < startOfNextMonth)
                                .Sum(x => (decimal?)x.OrderTotal) ?? 0
                })
                .FirstOrDefaultAsync();

    
            var totalProducts = await _unitOfWork.ProductRepository.GetAllAsync();

            return new DashboardStatsVM
            {
                TotalOrders = ordersData?.TotalOrders ?? 0,
                PendingOrders = ordersData?.Pending ?? 0,
                ApprovedOrders = ordersData?.Approved ?? 0,
                ProcessingOrders = ordersData?.Processing ?? 0,
                ShippedOrders = ordersData?.Shipped ?? 0,
                DeliveredOrders = ordersData?.Delivered ?? 0,
                CancelledOrders = ordersData?.Cancelled ?? 0,
                TotalCustomers = ordersData?.TotalCustomers ?? 0,
                TotalProducts = totalProducts.Count(),
                TotalRevenue = ordersData?.TotalRevenue ?? 0,
                TodayRevenue = ordersData?.TodayRevenue ?? 0,
                MonthlyRevenue = ordersData?.MonthlyRevenue ?? 0,
            };
        }
    }
}