using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.RequestModels
{
    public class OrderRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ReceiverName { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public int TotalPrice { get; set; }
    }
}
