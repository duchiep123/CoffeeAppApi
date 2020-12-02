using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coffee_App.RequestModels
{
    public class ResponseRegisterModel
    {
        public string Token { get; set; }
        public int Status { get; set; }
        public string Error { get; set; }
    }
}
