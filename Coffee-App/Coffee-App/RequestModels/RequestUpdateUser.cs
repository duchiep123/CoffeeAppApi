using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.RequestModels
{
    public class RequestUpdateUser
    {
        [Required]
        public string UserId { get; set; }

        public string  Fullname { get; set; }
        public string Phone { get; set; }
        public string Image { get; set; }
        public string address { get; set; }
    }
}
