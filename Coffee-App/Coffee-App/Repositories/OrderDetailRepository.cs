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
    }
}
