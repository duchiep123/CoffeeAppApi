using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class ProductSize
    {
        public ProductSize()
        {
            Product = new HashSet<Product>();
        }

        public string SizeId { get; set; }
        public int? PriceOfSizeS { get; set; }
        public int? PriceOfSizeM { get; set; }
        public int? PriceOfSizeL { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
