using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.RequestModels
{
    public class RequestBinder
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string ProviderId { get; set; }
        public string Image { get; set; }
      
    }
}
