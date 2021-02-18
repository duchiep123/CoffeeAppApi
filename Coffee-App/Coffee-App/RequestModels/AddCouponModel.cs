using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.RequestModels
{
    public class AddCouponModel
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string CouponId { get; set; }

    }
}
