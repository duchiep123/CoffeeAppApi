﻿using Coffee_App.GenericRepository;
using Coffee_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.IRepositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        int GetOrderIdByUserId(string userId);
        Order ConfirmOrder(int orderId, string userId);
        int CheckStatusOrder(int orderId);
        bool CheckCouponInOrder(string userId, string couponId);
        Task<List<string>> GetListCouponIdInOrderOfUser(string userId);
    }
}
