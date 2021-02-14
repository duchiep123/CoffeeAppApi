using Coffee_App.GenericRepository;
using Coffee_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.IRepositories
{
    public interface IOrderDetailRepository : IBaseRepository<OrderDetail>
    {
        int GetAmountByOrderId(int orderId);
        IQueryable<OrderDetail> GetOrderDetailsByOrderId(int orderId);
        int CheckOrderDetailBelongToOrder(int orderDetailId, int orderId);
        OrderDetail GetOrderDetailFollowOption(int orderId, string productId, string size);
    }
}
