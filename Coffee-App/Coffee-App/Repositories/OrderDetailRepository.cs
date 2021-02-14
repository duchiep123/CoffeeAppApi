using Coffee_App.GenericRepository;
using Coffee_App.IRepositories;
using Coffee_App.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.Repositories
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public int CheckOrderDetailBelongToOrder(int orderDetailId, int orderId)
        {
            OrderDetail orderDetail = (from o in _dbSet
                                       where o.OrderId == orderId && o.DetailId == orderDetailId
                                       select new OrderDetail {
                                           DetailId = o.DetailId
                                       }).FirstOrDefault<OrderDetail>();
            if(orderDetail != null)
            {
                return 1;
            }
            return -1;
                 
        }

        public int GetAmountByOrderId(int orderId)
        {
            IEnumerable<OrderDetail> enu = (from o in _dbSet
                                            where o.OrderId == orderId
                                            select new OrderDetail
                                            {
                                                OrderId = o.OrderId
                                            });
            List<OrderDetail> orderDetails = enu.ToList<OrderDetail>();
            if (orderDetails.Count != 0)
            {
                return orderDetails.Count;
            }
            return 0;

        }

        public OrderDetail GetOrderDetailFollowOption(int orderId, string productId, string size)
        {
            OrderDetail orderDetail = (from o in _dbSet
                                       where o.OrderId == orderId && o.ProductId == productId && o.Size == size
                                       select o).FirstOrDefault<OrderDetail>();
            return orderDetail;
        }

        public IQueryable<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            IQueryable<OrderDetail> orderDetails = (from o in _dbSet
                                            where o.OrderId == orderId
                                            select o);
            if (orderDetails.Count() != 0)
            {
                return orderDetails;
            }
            return null;
        }
    }
}
