using System;
using System.Collections.Generic;

namespace Coffee_App.Models
{
    public partial class RefreshToken
    {
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
