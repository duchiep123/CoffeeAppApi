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
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public bool CheckCounpoInOrder(string userId, string couponId)
        {
            Order order = (from o in _dbSet
                           where o.CouponId == couponId && o.UserId == userId
                           select o).FirstOrDefault<Order>();
            if (order != null)
            {
                return false;
            }
            return true;
        }

        public int CheckStatusOrder(int orderId)
        {
            Order order = (from o in _dbSet
                           where o.OrderId == orderId && o.Status == 0
                           select new Order
                           {
                               OrderId = o.OrderId

                           }).FirstOrDefault<Order>();
            if (order != null)
            {
                return 0;
            }
            return 1;
        }

        public Order ConfirmOrder(int orderId, string userId)
        {
            Order order = (from o in _dbSet
                           where o.OrderId == orderId && o.UserId == userId && o.Status == 0
                           select o).FirstOrDefault<Order>();
            return order;
        }

        public async Task<List<string>> GetListCouponIdInOrderOfUser(string userId)
        {
            List<Order> orders = await (from o in _dbSet
                                        where o.UserId == userId && o.CouponId != null
                                        select new Order
                                        {
                                            CouponId = o.CouponId

                                        }).ToListAsync<Order>();
            if (orders != null)
            {
                List<string> listCounpons = new List<string>();
                for (int i = 0; i < orders.Count; i++)
                {
                    listCounpons.Add(orders[i].CouponId);
                }
                return listCounpons;
            }
            return null;
        }

        public int GetOrderIdByUserId(string userId)
        {
            Order order = (from o in _dbSet
                           where o.UserId == userId && o.Status == 0
                           select new Order
                           {
                               OrderId = o.OrderId
                           }).FirstOrDefault<Order>();
            if (order != null)
            {
                return order.OrderId;
            }
            return -1;
        }


    }
}
