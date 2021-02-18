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
    public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<Coupon>> GetAvailableCoupon()
        {
            List<Coupon> coupons = await (from c in _dbSet
                                          where c.ExpiryDate >= DateTime.Now && c.Status == 1
                                          select new Coupon { 
                                            CouponId = c.CouponId,
                                            Title = c.Title,
                                            Description =c.Description,
                                            Image = c.Image,
                                            Sale = c.Sale,
                                            Condition = c.Condition,
                                            StartDate = c.StartDate,
                                            ExpiryDate = c.ExpiryDate
                                          }).ToListAsync<Coupon>();
            return coupons;
        }
    }
}
