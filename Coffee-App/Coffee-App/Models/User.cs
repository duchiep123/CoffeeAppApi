using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class User
    {
        public User()
        {
            Order = new HashSet<Order>();
        }

        public string UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ProviderId { get; set; }
        public string Image { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
