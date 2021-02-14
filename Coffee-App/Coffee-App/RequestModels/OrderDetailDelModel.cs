using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.RequestModels
{
    public class OrderDetailDelModel
    {
        [Required]
        public int OrderDetailId { get; set; }
        [Required]
        public int UnitPrice { get; set; }
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
