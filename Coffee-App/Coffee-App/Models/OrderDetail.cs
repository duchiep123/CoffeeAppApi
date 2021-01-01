using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class OrderDetail
    {
        public int DetailId { get; set; }
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public string Size { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
