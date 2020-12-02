using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class ProductType
    {
        public ProductType()
        {
            Product = new HashSet<Product>();
        }

        public int TypeId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
