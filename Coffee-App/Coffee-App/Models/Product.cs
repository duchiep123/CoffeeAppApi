using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int TypeId { get; set; }
        public string SizeId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime? CreateDate { get; set; }
        public int Status { get; set; }

        public virtual ProductSize Size { get; set; }
        public virtual ProductType Type { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
