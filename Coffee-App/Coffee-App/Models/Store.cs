using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class Store
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
    }
}
