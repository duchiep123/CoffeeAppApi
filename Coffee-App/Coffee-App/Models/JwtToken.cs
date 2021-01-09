using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class JwtToken
    {
        public string Token { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? RevokeDateTime { get; set; }
        public int Status { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
