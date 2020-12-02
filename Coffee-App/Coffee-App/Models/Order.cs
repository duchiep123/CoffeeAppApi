using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime? ReceiveTime { get; set; }
        public string CouponId { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }

        public virtual Coupon Coupon { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
