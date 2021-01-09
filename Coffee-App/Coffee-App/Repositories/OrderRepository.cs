﻿using Coffee_App.GenericRepository;
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