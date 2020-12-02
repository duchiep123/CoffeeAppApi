using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class Coupon
    {
        public Coupon()
        {
            Order = new HashSet<Order>();
        }

        public string CouponId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Sale { get; set; }
        public int Condition { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
